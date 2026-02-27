using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Livepasses.Sdk.Tests;

public class PassesResourceTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly LivepassesClient _client;

    public PassesResourceTests()
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

    [Fact]
    public async Task ShouldGenerateSinglePass()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/generate").UsingPost()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.PassGenerationSync())
        );

        var result = await _client.Passes.GenerateAsync(new Models.GeneratePassesParams
        {
            TemplateId = "template-001",
            Passes =
            [
                new Models.PassRecipient
                {
                    Customer = new Models.CustomerInfo { FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                    BusinessData = new Models.BusinessData { SectionInfo = "A", RowInfo = "12", SeatNumber = "5" }
                }
            ]
        });

        result.IsAsyncProcessing.Should().BeFalse();
        result.Passes.Should().HaveCount(1);
        result.Passes[0].Id.Should().Be("pass-001");
        result.BatchId.Should().Be("batch-001");
    }

    [Fact]
    public async Task ShouldGenerateAndWaitSync()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/generate").UsingPost()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.PassGenerationSync())
        );

        var result = await _client.Passes.GenerateAndWaitAsync(new Models.GeneratePassesParams
        {
            TemplateId = "template-001",
            Passes =
            [
                new Models.PassRecipient
                {
                    Customer = new Models.CustomerInfo { FirstName = "John", LastName = "Doe" },
                    BusinessData = new Models.BusinessData()
                }
            ]
        });

        result.IsAsyncProcessing.Should().BeFalse();
        result.Passes.Should().HaveCount(1);
    }

    [Fact]
    public async Task ShouldGenerateAndWaitAsyncWithPolling()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/generate").UsingPost()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.PassGenerationAsync())
        );

        // Poll returns completed immediately — tests the async generate → poll → merge flow
        _server.Given(
            Request.Create().WithPath("/api/passes/batch/batch-002/status").UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithBody(MockResponses.BatchStatusCompleted())
        );

        var progressValues = new List<double>();

        var result = await _client.Passes.GenerateAndWaitAsync(
            new Models.GeneratePassesParams
            {
                TemplateId = "template-001",
                Passes =
                [
                    new Models.PassRecipient
                    {
                        Customer = new Models.CustomerInfo { FirstName = "John", LastName = "Doe" },
                        BusinessData = new Models.BusinessData()
                    }
                ]
            },
            new Models.GenerateAndWaitOptions
            {
                PollInterval = 10,
                MaxAttempts = 10,
                OnProgress = status => progressValues.Add(status.ProgressPercentage)
            });

        result.BatchId.Should().Be("batch-002");
        result.Passes.Should().HaveCount(2);
        result.Passes[0].Id.Should().Be("pass-001");
        result.Passes[1].Id.Should().Be("pass-002");
        progressValues.Should().Contain(100.0);
    }

    [Fact]
    public async Task ShouldListPasses()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes").UsingGet()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.GlobalPassList())
        );

        var result = await _client.Passes.ListAsync(new Models.ListPassesParams { Page = 1, PageSize = 10 });

        result.Items.Should().HaveCount(1);
        result.Pagination.TotalItems.Should().Be(1);
    }

    [Fact]
    public async Task ShouldLookupPass()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/lookup").UsingGet()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.PassLookup())
        );

        var result = await _client.Passes.LookupAsync(new Models.LookupPassParams { PassId = "pass-001" });

        result.PassId.Should().Be("pass-001");
        result.IsValid.Should().BeTrue();
        result.CanBeRedeemed.Should().BeTrue();
        result.TemplateType.Should().Be("Event");
    }

    [Fact]
    public async Task ShouldValidatePass()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/pass-001/validate").UsingGet()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.PassValidation())
        );

        var result = await _client.Passes.ValidateAsync("pass-001");

        result.CanBeRedeemed.Should().BeTrue();
        result.VerificationMethods.Should().Contain("qr");
        result.VerificationMethods.Should().Contain("nfc");
    }

    [Fact]
    public async Task ShouldRedeemPass()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/pass-001/redeem").UsingPost()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.RedemptionResult())
        );

        var result = await _client.Passes.RedeemAsync("pass-001");

        result.NewStatus.Should().Be("Redeemed");
        result.AlreadyRedeemed.Should().BeFalse();
        result.PreviousStatus.Should().Be("Active");
    }

    [Fact]
    public async Task ShouldCheckInWithLocation()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/pass-001/check-in").UsingPost()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.RedemptionResult())
        );

        var result = await _client.Passes.CheckInAsync("pass-001", new Models.CheckInParams
        {
            Location = new Models.RedemptionLocation { Name = "Gate A", Latitude = 4.6097, Longitude = -74.0817 },
            Notes = "VIP entrance"
        });

        result.PassId.Should().Be("pass-001");

        var body = _server.LogEntries.First().RequestMessage.Body!;
        body.Should().Contain("\"location\"");
        body.Should().Contain("\"latitude\"");
    }

    [Fact]
    public async Task ShouldEarnLoyaltyPoints()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/pass-001/loyalty/transact").UsingPost()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.RedemptionResult())
        );

        var result = await _client.Passes.LoyaltyTransactAsync("pass-001", new Models.LoyaltyTransactionParams
        {
            TransactionType = "earn",
            Points = 500,
            Description = "Purchase reward"
        });

        result.PassId.Should().Be("pass-001");

        var body = _server.LogEntries.First().RequestMessage.Body!;
        body.Should().Contain("\"transactionType\"");
        body.Should().Contain("\"earn\"");
        body.Should().Contain("500");
    }

    [Fact]
    public async Task ShouldGetBatchStatus()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/batch/batch-001/status").UsingGet()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.BatchStatus())
        );

        var result = await _client.Passes.GetBatchStatusAsync("batch-001");

        result.IsCompleted.Should().BeTrue();
        result.PassesGenerated.Should().Be(5);
        result.ProgressPercentage.Should().Be(100.0);
    }
}
