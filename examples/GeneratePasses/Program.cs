// Example: End-to-end pass generation, lookup, validation, and check-in
// Run: dotnet run

using Livepasses.Sdk;
using Livepasses.Sdk.Exceptions;
using Livepasses.Sdk.Models;

var apiKey = Environment.GetEnvironmentVariable("LIVEPASSES_API_KEY") ?? "";
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("Set LIVEPASSES_API_KEY environment variable");
    Console.WriteLine("Get your key at https://dashboard.livepasses.com/api-keys");
    return;
}

var client = new LivepassesClient(apiKey);

try
{
    // 1. List templates
    Console.WriteLine("=== Templates ===");
    var templates = await client.Templates.ListAsync();
    foreach (var t in templates.Items)
        Console.WriteLine($"  {t.Id}: {t.Name} ({t.Type})");

    if (templates.Items.Count == 0)
    {
        Console.WriteLine("No templates found. Create one first.");
        return;
    }

    var templateId = templates.Items[0].Id;

    // 2. Generate event pass with auto-polling
    Console.WriteLine("\n=== Generate Pass ===");
    var result = await client.Passes.GenerateAndWaitAsync(
        new GeneratePassesParams
        {
            TemplateId = templateId,
            BusinessContext = new BusinessContext
            {
                Event = new EventContext
                {
                    EventName = "Tech Conference 2026",
                    EventDate = "2026-06-15T09:00:00Z",
                    DoorsOpen = "2026-06-15T08:00:00Z"
                }
            },
            Passes =
            [
                new PassRecipient
                {
                    Customer = new CustomerInfo
                    {
                        FirstName = "Alice", LastName = "Smith", Email = "alice@example.com"
                    },
                    BusinessData = new BusinessData
                    {
                        SectionInfo = "VIP", RowInfo = "A", SeatNumber = "1",
                        TicketType = "VIP", Price = 150.00m, Currency = "USD"
                    }
                }
            ],
            Options = new PassGenerationOptions { DeliveryMethod = "email" }
        },
        new GenerateAndWaitOptions
        {
            OnProgress = status => Console.WriteLine($"  Progress: {status.ProgressPercentage}%")
        });

    Console.WriteLine($"Generated {result.TotalPasses} pass(es)");
    foreach (var pass in result.Passes)
    {
        Console.WriteLine($"  Pass {pass.Id}: Apple={pass.Platforms.Apple.Available}, Google={pass.Platforms.Google.Available}");
    }

    // 3. Validate
    if (result.Passes.Count > 0)
    {
        var passId = result.Passes[0].Id;

        Console.WriteLine("\n=== Validate ===");
        var validation = await client.Passes.ValidateAsync(passId);
        Console.WriteLine($"  Can redeem: {validation.CanBeRedeemed}");

        // 4. Check in
        if (validation.CanBeRedeemed)
        {
            Console.WriteLine("\n=== Check In ===");
            var redemption = await client.Passes.CheckInAsync(passId, new CheckInParams
            {
                Location = new RedemptionLocation
                {
                    Name = "Main Gate", Latitude = 40.71, Longitude = -74.00
                }
            });
            Console.WriteLine($"  {redemption.PreviousStatus} -> {redemption.NewStatus}");
        }
    }

    Console.WriteLine("\nDone!");
}
catch (AuthenticationException)
{
    Console.Error.WriteLine("Invalid API key");
}
catch (ValidationException ex)
{
    Console.Error.WriteLine($"Validation error: {ex.Message}");
}
catch (RateLimitException ex)
{
    Console.Error.WriteLine($"Rate limited. Retry after {ex.RetryAfter}s");
}
catch (NotFoundException)
{
    Console.Error.WriteLine("Resource not found");
}
catch (BusinessRuleException ex)
{
    Console.Error.WriteLine($"Business rule: {ex.Message}");
}
catch (LivepassesException ex)
{
    Console.Error.WriteLine($"API error [{ex.Code}]: {ex.Message}");
}
