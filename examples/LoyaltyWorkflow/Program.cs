// Example: Loyalty card lifecycle — generate, earn points, spend points, update tier
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
    // 1. Generate a loyalty card
    Console.WriteLine("Generating loyalty card...");
    var templateId = Environment.GetEnvironmentVariable("LOYALTY_TEMPLATE_ID") ?? "loyalty-template-id";
    var result = await client.Passes.GenerateAndWaitAsync(
        new GeneratePassesParams
        {
            TemplateId = templateId,
            Passes =
            [
                new PassRecipient
                {
                    Customer = new CustomerInfo
                    {
                        FirstName = "Carlos", LastName = "Rivera",
                        Email = "carlos@example.com", Phone = "+57300123456"
                    },
                    BusinessData = new BusinessData
                    {
                        MembershipNumber = "MEM-2026-001",
                        CurrentPoints = 0, MemberTier = "Bronze", AccountBalance = 0
                    }
                }
            ]
        },
        new GenerateAndWaitOptions
        {
            OnProgress = s => Console.WriteLine($"  Progress: {s.ProgressPercentage}%")
        });

    var passId = result.Passes[0].Id;
    Console.WriteLine($"  Pass ID: {passId}\n");

    // 2. Earn points from a purchase
    Console.WriteLine("Earning 500 points from purchase...");
    await client.Passes.LoyaltyTransactAsync(passId, new LoyaltyTransactionParams
    {
        TransactionType = "earn", Points = 500, Description = "Purchase at Store #42 - $50.00"
    });

    // 3. Earn more points
    Console.WriteLine("Earning 300 more points...");
    await client.Passes.LoyaltyTransactAsync(passId, new LoyaltyTransactionParams
    {
        TransactionType = "earn", Points = 300, Description = "Purchase at Store #15 - $30.00"
    });

    // 4. Spend points for a reward
    Console.WriteLine("Spending 200 points on a reward...");
    var spend = await client.Passes.LoyaltyTransactAsync(passId, new LoyaltyTransactionParams
    {
        TransactionType = "spend", Points = 200, Description = "Redeemed: Free coffee"
    });
    Console.WriteLine($"  Status: {spend.NewStatus}\n");

    // 5. Update tier
    Console.WriteLine("Upgrading to Gold tier...");
    await client.Passes.UpdateAsync(passId, new UpdatePassParams
    {
        BusinessData = new BusinessData { CurrentPoints = 600, MemberTier = "Gold" },
        BusinessContext = new BusinessContext
        {
            Loyalty = new LoyaltyContext
            {
                ProgramUpdate = "Congratulations! You've been upgraded to Gold tier!",
                SeasonalMessage = "Enjoy double points this month!"
            }
        }
    });
    Console.WriteLine("  Tier updated to Gold\n");

    // 6. Validate the pass
    Console.WriteLine("Validating pass...");
    var validation = await client.Passes.ValidateAsync(passId);
    Console.WriteLine($"  Valid: {validation.CanBeRedeemed}");
    Console.WriteLine($"  Message: {validation.ValidationMessage}\n");

    Console.WriteLine("Done!");
}
catch (BusinessRuleException ex)
{
    Console.Error.WriteLine($"Business rule violation: {ex.Message}");
}
catch (LivepassesException ex)
{
    Console.Error.WriteLine($"API error [{ex.Code}]: {ex.Message}");
}
