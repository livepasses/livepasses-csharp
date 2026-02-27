using Livepasses.Sdk.Internal;
using Livepasses.Sdk.Models;

namespace Livepasses.Sdk.Resources;

/// <summary>
/// Resource for managing webhooks.
/// </summary>
public class WebhooksResource
{
    private readonly LivepassesHttpClient _http;

    internal WebhooksResource(LivepassesHttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Create a new webhook.
    /// </summary>
    public async Task<Webhook> CreateAsync(CreateWebhookParams parameters)
    {
        return await _http.PostAsync<Webhook>("/api/webhooks", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// List all webhooks.
    /// </summary>
    public async Task<List<Webhook>> ListAsync()
    {
        return await _http.GetAsync<List<Webhook>>("/api/webhooks").ConfigureAwait(false);
    }

    /// <summary>
    /// Delete a webhook by ID.
    /// </summary>
    public async Task DeleteAsync(string webhookId)
    {
        await _http.DeleteAsync($"/api/webhooks/{webhookId}").ConfigureAwait(false);
    }
}
