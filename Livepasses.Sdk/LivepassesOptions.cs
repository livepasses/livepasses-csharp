namespace Livepasses.Sdk;

/// <summary>
/// Configuration options for the Livepasses client.
/// </summary>
public class LivepassesOptions
{
    /// <summary>Base URL for the Livepasses API. Default: https://api.livepasses.com</summary>
    public string BaseUrl { get; set; } = "https://api.livepasses.com";

    /// <summary>Request timeout. Default: 30 seconds.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>Maximum number of retries for failed requests. Default: 3.</summary>
    public int MaxRetries { get; set; } = 3;
}
