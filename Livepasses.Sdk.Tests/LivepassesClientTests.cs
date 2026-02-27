using FluentAssertions;

namespace Livepasses.Sdk.Tests;

public class LivepassesClientTests
{
    [Fact]
    public void ShouldThrowOnEmptyApiKey()
    {
        var act = () => new LivepassesClient("");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldThrowOnNullApiKey()
    {
        var act = () => new LivepassesClient(null!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldThrowOnWhitespaceApiKey()
    {
        var act = () => new LivepassesClient("   ");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldCreateResourceInstances()
    {
        var client = new LivepassesClient("lp_test_key");

        client.Passes.Should().NotBeNull();
        client.Templates.Should().NotBeNull();
        client.Webhooks.Should().NotBeNull();
    }

    [Fact]
    public void ShouldAcceptCustomOptions()
    {
        var act = () => new LivepassesClient("lp_test_key", new LivepassesOptions
        {
            BaseUrl = "https://custom.api.com",
            Timeout = TimeSpan.FromSeconds(60),
            MaxRetries = 5
        });

        act.Should().NotThrow();
    }

    [Fact]
    public void ShouldUseDefaultOptions()
    {
        var act = () => new LivepassesClient("lp_test_key");
        act.Should().NotThrow();
    }
}
