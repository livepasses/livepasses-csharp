using Livepasses.Sdk.Internal;
using Livepasses.Sdk.Resources;

namespace Livepasses.Sdk;

/// <summary>
/// Official C# client for the Livepasses API.
/// </summary>
public class LivepassesClient
{
    /// <summary>Pass generation, lookup, validation, and redemption.</summary>
    public PassesResource Passes { get; }

    /// <summary>Template management.</summary>
    public TemplatesResource Templates { get; }

    /// <summary>Webhook management.</summary>
    public WebhooksResource Webhooks { get; }

    /// <summary>
    /// Creates a new Livepasses client.
    /// </summary>
    /// <param name="apiKey">Your Livepasses API key.</param>
    /// <param name="options">Optional configuration overrides.</param>
    /// <exception cref="ArgumentException">Thrown when the API key is null or empty.</exception>
    public LivepassesClient(string apiKey, LivepassesOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey,
            "API key is required. Get yours at https://dashboard.livepasses.com/api-keys");

        options ??= new LivepassesOptions();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl.TrimEnd('/')),
            Timeout = options.Timeout
        };

        var internalClient = new LivepassesHttpClient(httpClient, apiKey, options.MaxRetries);

        Passes = new PassesResource(internalClient);
        Templates = new TemplatesResource(internalClient);
        Webhooks = new WebhooksResource(internalClient);
    }
}
