// Example: Register, list, and manage webhooks
// Run: dotnet run

using Livepasses.Sdk;
using Livepasses.Sdk.Exceptions;
using Livepasses.Sdk.Models;

var apiKey = Environment.GetEnvironmentVariable("LIVEPASSES_API_KEY") ?? "";
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("Set LIVEPASSES_API_KEY environment variable");
    return;
}

var client = new LivepassesClient(apiKey);

try
{
    // 1. Create a webhook for pass events
    Console.WriteLine("Registering webhook...");
    var webhook = await client.Webhooks.CreateAsync(new CreateWebhookParams
    {
        Url = "https://your-app.com/webhooks/livepasses",
        Events = [
            WebhookEventType.PassGenerated,
            WebhookEventType.PassRedeemed,
            WebhookEventType.PassUpdated,
            WebhookEventType.TransferAccepted
        ]
    });
    Console.WriteLine($"  Webhook ID: {webhook.Id}");
    Console.WriteLine($"  URL: {webhook.Url}");
    Console.WriteLine($"  Events: {string.Join(", ", webhook.Events)}");
    Console.WriteLine($"  Secret: {webhook.Secret}");
    Console.WriteLine("  (Store this secret to verify incoming webhook signatures)\n");

    // 2. List all registered webhooks
    Console.WriteLine("Listing all webhooks...");
    var webhooks = await client.Webhooks.ListAsync();
    foreach (var wh in webhooks)
    {
        Console.WriteLine($"  - {wh.Id}: {wh.Url} (active: {wh.IsActive})");
        Console.WriteLine($"    Events: {string.Join(", ", wh.Events)}");
    }
    Console.WriteLine();

    // 3. Clean up — delete the webhook
    Console.WriteLine("Deleting webhook...");
    await client.Webhooks.DeleteAsync(webhook.Id);
    Console.WriteLine("  Webhook deleted\n");

    Console.WriteLine("Done!");
}
catch (LivepassesException ex)
{
    Console.Error.WriteLine($"API error [{ex.Code}]: {ex.Message}");
}
