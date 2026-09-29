using FluentAssertions;
using Livepasses.Sdk.Exceptions;
using Livepasses.Sdk.Internal;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Livepasses.Sdk.Tests;

public class HttpClientTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly LivepassesClient _client;

    // Direct access to the internal client (no public-client wrapper) for tests that
    // assert on the raw request/response contract: status-driven typed errors and the
    // 5xx-retry-gate-by-method rule. Named `_httpClient`/`_retryingClient` rather than
    // reusing `_client` because that field is already the public `LivepassesClient`.
    private readonly LivepassesHttpClient _httpClient;
    private readonly LivepassesHttpClient _retryingClient;

    public HttpClientTests()
    {
        _server = WireMockServer.Start();
        _client = new LivepassesClient("lp_test_key", new LivepassesOptions
        {
            BaseUrl = _server.Url!,
            MaxRetries = 0,
            Timeout = TimeSpan.FromSeconds(10)
        });

        _httpClient = new LivepassesHttpClient(
            new HttpClient { BaseAddress = new Uri(_server.Url!), Timeout = TimeSpan.FromSeconds(10) },
            "lp_test_key",
            maxRetries: 0);

        _retryingClient = new LivepassesHttpClient(
            new HttpClient { BaseAddress = new Uri(_server.Url!), Timeout = TimeSpan.FromSeconds(10) },
            "lp_test_key",
            maxRetries: 3);
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task ShouldSendApiKeyHeader()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/lookup").UsingGet()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.PassLookup())
        );

        await _client.Passes.LookupAsync(new Models.LookupPassParams { PassId = "pass-001" });

        _server.LogEntries.Should().ContainSingle();
        _server.LogEntries.First().RequestMessage.Headers!["X-API-Key"].Should().Contain("lp_test_key");
    }

    [Fact]
    public async Task ShouldSendPostWithJsonBody()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/generate").UsingPost()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.PassGenerationSync())
        );

        await _client.Passes.GenerateAsync(new Models.GeneratePassesParams
        {
            TemplateId = "template-001",
            Passes =
            [
                new Models.PassRecipient
                {
                    Customer = new Models.CustomerInfo { FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                    BusinessData = new Models.BusinessData { SectionInfo = "A", RowInfo = "12" }
                }
            ]
        });

        var entry = _server.LogEntries.First();
        entry.RequestMessage.Headers!["Content-Type"].Should().Contain("application/json; charset=utf-8");

        var body = entry.RequestMessage.Body!;
        body.Should().Contain("\"templateId\"");
        body.Should().Contain("\"passes\"");
        body.Should().Contain("\"firstName\"");
    }

    [Fact]
    public async Task ShouldSerializeQueryParams()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes").UsingGet()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.GlobalPassList())
        );

        await _client.Passes.ListAsync(new Models.ListPassesParams
        {
            Page = 1,
            PageSize = 10,
            TemplateId = "template-001"
        });

        var url = _server.LogEntries.First().RequestMessage.Url;
        url.Should().Contain("page=1");
        url.Should().Contain("pageSize=10");
        url.Should().Contain("templateId=template-001");
    }

    [Fact]
    public async Task ShouldUnwrapApiResponse()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/lookup").UsingGet()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.PassLookup())
        );

        var result = await _client.Passes.LookupAsync(new Models.LookupPassParams { PassId = "pass-001" });

        result.PassId.Should().Be("pass-001");
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldThrowAuthenticationException()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/lookup").UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(401)
                .WithBody(MockResponses.ApiError("UNAUTHORIZED", "Invalid API key"))
        );

        var act = () => _client.Passes.LookupAsync(new Models.LookupPassParams { PassId = "pass-001" });

        var ex = await act.Should().ThrowAsync<AuthenticationException>();
        ex.Which.Code.Should().Be("UNAUTHORIZED");
        ex.Which.Status.Should().Be(401);
    }

    [Fact]
    public async Task ShouldThrowNotFoundException()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/lookup").UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(404)
                .WithBody(MockResponses.ApiError("NOT_FOUND", "Pass not found"))
        );

        var act = () => _client.Passes.LookupAsync(new Models.LookupPassParams { PassId = "invalid" });

        var ex = await act.Should().ThrowAsync<NotFoundException>();
        ex.Which.Code.Should().Be("NOT_FOUND");
        ex.Which.Status.Should().Be(404);
    }

    [Fact]
    public async Task ShouldThrowValidationException()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/generate").UsingPost()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(400)
                .WithBody(MockResponses.ApiError("VALIDATION_ERROR", "Missing required field"))
        );

        var act = () => _client.Passes.GenerateAsync(new Models.GeneratePassesParams
        {
            TemplateId = "template-001",
            Passes = []
        });

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Code.Should().Be("VALIDATION_ERROR");
        ex.Which.Status.Should().Be(400);
    }

    [Fact]
    public async Task ShouldThrowRateLimitExceptionWithRetryAfter()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes/lookup").UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(429)
                .WithHeader("Retry-After", "30")
                .WithBody(MockResponses.ApiError("RATE_LIMIT_EXCEEDED", "Too many requests"))
        );

        var act = () => _client.Passes.LookupAsync(new Models.LookupPassParams { PassId = "pass-001" });

        var ex = await act.Should().ThrowAsync<RateLimitException>();
        ex.Which.Code.Should().Be("RATE_LIMIT_EXCEEDED");
        ex.Which.Status.Should().Be(429);
    }

    [Fact]
    public async Task ShouldRetryOn5xxErrors()
    {
        // Client with 2 retries (3 total attempts, capped at 3 for 5xx)
        var retryClient = new LivepassesClient("lp_test_key", new LivepassesOptions
        {
            BaseUrl = _server.Url!,
            MaxRetries = 2,
            Timeout = TimeSpan.FromSeconds(10)
        });

        // Always return 500 — verify the client retries multiple times
        _server.Given(
            Request.Create().WithPath("/api/passes/pass-001/validate").UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(500)
                .WithBody(MockResponses.ApiError("INTERNAL_ERROR", "Server error"))
        );

        // The final response is 500, which should be unwrapped as a LivepassesException
        var act = () => retryClient.Passes.ValidateAsync("pass-001");
        await act.Should().ThrowAsync<LivepassesException>();

        // Verify that the client retried (should have made multiple requests)
        _server.LogEntries.Count().Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task ShouldParsePagedResponse()
    {
        _server.Given(
            Request.Create().WithPath("/api/passes").UsingGet()
        ).RespondWith(
            Response.Create().WithStatusCode(200).WithBody(MockResponses.GlobalPassList())
        );

        var result = await _client.Passes.ListAsync();

        result.Items.Should().HaveCount(1);
        result.Pagination.TotalItems.Should().Be(1);
        result.Pagination.CurrentPage.Should().Be(1);
    }

    [Fact]
    public async Task RefusalWithStatus_RaisesTypedErrorFromEnvelope()
    {
        _server.Given(Request.Create().WithPath("/api/templates/x").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(404)
                .WithBody("""{"success":false,"data":null,"error":{"code":"TEMPLATE_NOT_FOUND","message":"gone"}}"""));
        var act = () => _httpClient.GetAsync<object>("/api/templates/x");
        (await act.Should().ThrowAsync<NotFoundException>()).Which.Code.Should().Be("TEMPLATE_NOT_FOUND");
    }

    [Fact]
    public async Task ValidationError_CarriesFields()
    {
        _server.Given(Request.Create().WithPath("/api/x").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(400)
                .WithBody("""{"success":false,"error":{"code":"VALIDATION_ERROR","message":"bad","fields":{"name":["required"]}}}"""));
        var act = () => _httpClient.PostAsync<object>("/api/x", new { });
        (await act.Should().ThrowAsync<ValidationException>()).Which.Fields!["name"].Should().Equal("required");
    }

    [Fact]
    public async Task ForbiddenStatus_WinsOverAnUnauthorizedCode()
    {
        // A handler-level UNAUTHORIZED refusal answers 403: a permission problem, not a bad key.
        _server.Given(Request.Create().WithPath("/api/x").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(403)
                .WithBody("""{"success":false,"data":null,"error":{"code":"UNAUTHORIZED","message":"no"}}"""));
        var act = () => _httpClient.GetAsync<object>("/api/x");
        var ex = (await act.Should().ThrowAsync<ForbiddenException>()).Which;
        ex.Status.Should().Be(403);
        ex.Code.Should().Be("UNAUTHORIZED");
    }

    [Fact]
    public async Task QuotaExceeded_CarriesTheRealStatus()
    {
        _server.Given(Request.Create().WithPath("/api/templates").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(422)
                .WithBody("""{"success":false,"data":null,"error":{"code":"QUOTA_EXCEEDED","message":"limit"}}"""));
        var act = () => _httpClient.PostAsync<object>("/api/templates", new { });
        (await act.Should().ThrowAsync<QuotaExceededException>()).Which.Status.Should().Be(422);
    }

    [Fact]
    public async Task EmptyBody401_IsAuthenticationException()
    {
        _server.Given(Request.Create().WithPath("/api/x").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(401));
        var act = () => _httpClient.GetAsync<object>("/api/x");
        (await act.Should().ThrowAsync<AuthenticationException>()).Which.Status.Should().Be(401);
    }

    [Fact]
    public void TypedExceptionConstructors_KeepTheirOldDefaultStatus()
    {
        new QuotaExceededException("m", "QUOTA_EXCEEDED").Status.Should().Be(403);
        new ForbiddenException("m", "FORBIDDEN", null, 451).Status.Should().Be(451);
    }

    [Fact]
    public async Task EmptyBodyError_RaisesTypedError()
    {
        _server.Given(Request.Create().WithPath("/api/webhooks/x").UsingDelete())
            .RespondWith(Response.Create().WithStatusCode(404));
        var act = () => _httpClient.DeleteAsync("/api/webhooks/x");
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task RetriesServerErrorOnlyForIdempotentMethods()
    {
        const string body = """{"success":false,"error":{"code":"EXTERNAL_SERVICE_ERROR","message":"upstream"}}""";
        _server.Given(Request.Create().UsingAnyMethod()).RespondWith(Response.Create().WithStatusCode(502).WithBody(body));

        await ((Func<Task>)(() => _retryingClient.PostAsync<object>("/api/passes/generate", new { }))).Should().ThrowAsync<LivepassesException>();
        _server.LogEntries.Should().HaveCount(1);

        await ((Func<Task>)(() => _retryingClient.GetAsync<object>("/api/passes/x"))).Should().ThrowAsync<LivepassesException>();
        _server.LogEntries.Should().HaveCount(4);
    }

    [Fact]
    public async Task PagedRefusalWithStatus_RaisesTypedErrorFromEnvelope()
    {
        _server.Given(Request.Create().WithPath("/api/passes").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(403)
                .WithBody("""{"success":false,"data":null,"error":{"code":"FORBIDDEN","message":"no access"}}"""));
        var act = () => _httpClient.GetPagedAsync<object>("/api/passes");
        (await act.Should().ThrowAsync<ForbiddenException>()).Which.Code.Should().Be("FORBIDDEN");
    }
}
