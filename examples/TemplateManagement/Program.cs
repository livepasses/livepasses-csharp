// Example: CRUD operations on pass templates
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
    // 1. Create a new event template
    Console.WriteLine("Creating event template...");
    var template = await client.Templates.CreateAsync(new CreateTemplateParams
    {
        Name = "VIP Concert Pass",
        Description = "Premium concert ticket with VIP access",
        BusinessFeatures = new Dictionary<string, object>
        {
            ["passType"] = "event",
            ["hasSeating"] = true,
            ["hasGateInfo"] = true,
            ["supportedPlatforms"] = new[] { "apple", "google" }
        }
    });
    Console.WriteLine($"  Created: {template.Id} — \"{template.Name}\"");
    Console.WriteLine($"  Status: {template.Status}\n");

    // 2. Update the template
    Console.WriteLine("Updating template...");
    var updated = await client.Templates.UpdateAsync(template.Id, new UpdateTemplateParams
    {
        Name = "VIP Concert Pass v2",
        Description = "Updated premium concert ticket with backstage access"
    });
    Console.WriteLine($"  Updated: \"{updated.Name}\"\n");

    // 3. Activate the template
    Console.WriteLine("Activating template...");
    await client.Templates.ActivateAsync(template.Id);
    Console.WriteLine("  Template is now active\n");

    // 4. List all active templates
    Console.WriteLine("Listing active templates...");
    var templates = await client.Templates.ListAsync(new ListTemplatesParams { Status = "Active" });
    foreach (var t in templates.Items)
        Console.WriteLine($"  - {t.Name} ({t.Type}) [{t.Status}]");
    Console.WriteLine($"  Total: {templates.Pagination.TotalItems}\n");

    // 5. Get template details
    Console.WriteLine("Getting template details...");
    var detail = await client.Templates.GetAsync(template.Id);
    Console.WriteLine($"  Name: {detail.Name}");
    Console.WriteLine($"  Type: {detail.Type}\n");

    // 6. Deactivate when done
    Console.WriteLine("Deactivating template...");
    await client.Templates.DeactivateAsync(template.Id);
    Console.WriteLine("  Template deactivated\n");

    Console.WriteLine("Done!");
}
catch (ValidationException ex)
{
    Console.Error.WriteLine($"Validation error: {ex.Message}");
}
catch (LivepassesException ex)
{
    Console.Error.WriteLine($"API error [{ex.Code}]: {ex.Message}");
}
