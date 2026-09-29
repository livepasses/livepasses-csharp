using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Livepasses.Sdk.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Livepasses.Sdk.Tests;

/// <summary>
/// The API answers 400 for any body field its request DTO does not declare (#797). Every
/// property these params serialize must therefore be a field the server declares, by wire name.
/// </summary>
public class RequestBodyContractTests
{
    public static TheoryData<Type, string[]> Contracts() => new()
    {
        { typeof(UpdatePassParams), ["updatedFields", "reason", "messageHeader", "messageBody", "notify"] },
        { typeof(RedeemPassParams), ["acceptedTypes", "redemptionMethod", "redemptionChannel", "location", "confirmationCode", "metadata"] },
        { typeof(CheckInParams), ["acceptedTypes", "gate", "section", "redemptionMethod", "location", "metadata"] },
        { typeof(RedeemCouponParams), ["acceptedTypes", "redemptionChannel", "location", "locationId", "transactionAmount", "transactionCurrency", "promoCode", "redemptionMethod", "metadata"] },
        { typeof(RedemptionLocation), ["name", "latitude", "longitude"] },
    };

    [Theory]
    [MemberData(nameof(Contracts))]
    public void SerializesOnlyFieldsTheServerDeclares(Type paramsType, string[] declared)
    {
        var wireNames = paramsType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() is null)
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                         ?? JsonNamingPolicy.CamelCase.ConvertName(p.Name));

        wireNames.Should().BeSubsetOf(declared,
            "the API refuses any body field {0} sends that the server does not declare", paramsType.Name);
    }
}

public class RequestBodyWireTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly LivepassesClient _client;

    public RequestBodyWireTests()
    {
        _server = WireMockServer.Start();
        _client = new LivepassesClient("lp_test_key", new LivepassesOptions
        {
            BaseUrl = _server.Url!,
            MaxRetries = 0,
            Timeout = TimeSpan.FromSeconds(10)
        });
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }

    private JsonElement SentBody() =>
        JsonDocument.Parse(_server.LogEntries.Single().RequestMessage.Body!).RootElement;

    private static string[] KeysOf(JsonElement element) =>
        element.EnumerateObject().Select(p => p.Name).ToArray();

    [Fact]
    public async Task UpdateSendsOnlyTheDeclaredUpdateContract()
    {
        _server.Given(Request.Create().WithPath("/api/passes/pass-001").UsingPut())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{\"success\":true,\"data\":{}}"));

        await _client.Passes.UpdateAsync("pass-001", new UpdatePassParams
        {
            UpdatedFields = new Dictionary<string, object> { ["points"] = 600, ["memberTier"] = "Gold" },
            Reason = "Purchase reward",
            MessageHeader = "Points added",
            MessageBody = "You earned 100 points",
            Notify = true
        });

        var body = SentBody();
        KeysOf(body).Should().BeEquivalentTo("updatedFields", "reason", "messageHeader", "messageBody", "notify");
        body.GetProperty("updatedFields").GetProperty("points").GetInt32().Should().Be(600);
        body.GetProperty("updatedFields").GetProperty("memberTier").GetString().Should().Be("Gold");
        body.GetProperty("notify").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task UpdateOmitsUnsetOptionalFields()
    {
        _server.Given(Request.Create().WithPath("/api/passes/pass-001").UsingPut())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{\"success\":true,\"data\":{}}"));

        await _client.Passes.UpdateAsync("pass-001", new UpdatePassParams
        {
            UpdatedFields = new Dictionary<string, object> { ["validUntil"] = "2027-01-31" }
        });

        KeysOf(SentBody()).Should().BeEquivalentTo("updatedFields");
    }

    [Fact]
    public async Task RedeemFamilyBodiesCarryMetadataAndNoNotes()
    {
        foreach (var route in new[] { "redeem", "check-in", "redeem-coupon" })
        {
            _server.Given(Request.Create().WithPath($"/api/passes/pass-001/{route}").UsingPost())
                .RespondWith(Response.Create().WithStatusCode(200).WithBody(MockResponses.RedemptionResult()));
        }

        var metadata = new Dictionary<string, string> { ["note"] = "Applied to order #12345" };
        await _client.Passes.RedeemAsync("pass-001", new RedeemPassParams { Metadata = metadata });
        await _client.Passes.CheckInAsync("pass-001", new CheckInParams { Metadata = metadata });
        await _client.Passes.RedeemCouponAsync("pass-001", new RedeemCouponParams { Metadata = metadata });

        _server.LogEntries.Should().HaveCount(3);
        foreach (var entry in _server.LogEntries)
        {
            var body = JsonDocument.Parse(entry.RequestMessage.Body!).RootElement;
            KeysOf(body).Should().BeEquivalentTo("metadata");
            body.GetProperty("metadata").GetProperty("note").GetString().Should().Be("Applied to order #12345");
        }
    }
}
