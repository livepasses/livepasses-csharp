namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for creating a webhook.
/// </summary>
public class CreateWebhookParams
{
    public required string Url { get; set; }
    public required List<string> Events { get; set; }
}
