// Example: Coupon pass generation and redemption
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
    // 1. Generate coupon passes for a campaign
    Console.WriteLine("Generating coupon passes...");
    var templateId = Environment.GetEnvironmentVariable("COUPON_TEMPLATE_ID") ?? "coupon-template-id";
    var result = await client.Passes.GenerateAndWaitAsync(
        new GeneratePassesParams
        {
            TemplateId = templateId,
            BusinessContext = new BusinessContext
            {
                Coupon = new CouponContext
                {
                    CampaignName = "Summer Sale 2026",
                    SpecialMessage = "Enjoy 20% off your next purchase!",
                    PromotionStartDate = "2026-06-01",
                    PromotionEndDate = "2026-08-31"
                }
            },
            Passes =
            [
                new PassRecipient
                {
                    Customer = new CustomerInfo { FirstName = "Maria", LastName = "Garcia", Email = "maria@example.com" },
                    BusinessData = new BusinessData { PromoCode = "SUMMER20-001", MaxUsageCount = 1 }
                },
                new PassRecipient
                {
                    Customer = new CustomerInfo { FirstName = "Pedro", LastName = "Lopez", Email = "pedro@example.com" },
                    BusinessData = new BusinessData { PromoCode = "SUMMER20-002", MaxUsageCount = 1 }
                }
            ],
            Options = new PassGenerationOptions { DeliveryMethod = "email" }
        },
        new GenerateAndWaitOptions
        {
            OnProgress = s => Console.WriteLine($"  Progress: {s.ProgressPercentage}%")
        });

    Console.WriteLine($"Generated {result.TotalPasses} coupon(s)\n");

    // 2. Look up a coupon
    var passId = result.Passes[0].Id;
    Console.WriteLine("Looking up coupon...");
    var lookup = await client.Passes.LookupAsync(new LookupPassParams { PassId = passId });
    Console.WriteLine($"  Holder: {lookup.HolderName}");
    Console.WriteLine($"  Status: {lookup.Status}\n");

    // 3. Validate before redemption
    Console.WriteLine("Validating coupon...");
    var validation = await client.Passes.ValidateAsync(passId);
    Console.WriteLine($"  Can redeem: {validation.CanBeRedeemed}\n");

    // 4. Redeem the coupon at a store
    if (validation.CanBeRedeemed)
    {
        Console.WriteLine("Redeeming coupon...");
        try
        {
            var redemption = await client.Passes.RedeemCouponAsync(passId, new RedeemCouponParams
            {
                Location = new RedemptionLocation { Name = "Store #42", Latitude = 4.6097, Longitude = -74.0817 },
                Notes = "Applied to order #12345"
            });
            Console.WriteLine($"  Previous: {redemption.PreviousStatus}");
            Console.WriteLine($"  New: {redemption.NewStatus}");
            Console.WriteLine($"  At: {redemption.RedeemedAt}\n");
        }
        catch (BusinessRuleException ex)
        {
            Console.WriteLine($"  Cannot redeem: {ex.Message}\n");
        }
    }

    Console.WriteLine("Done!");
}
catch (LivepassesException ex)
{
    Console.Error.WriteLine($"API error [{ex.Code}]: {ex.Message}");
}
