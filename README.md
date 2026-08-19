# Livepasses C# SDK

Official C# SDK for the [Livepasses API](https://docs.livepasses.com). Generate Apple Wallet and Google Wallet passes with a simple, type-safe API.

## Requirements

- .NET 8.0 or later
- Zero runtime dependencies

## Installation

```bash
dotnet add package Livepasses
```

## Quick Start

```csharp
using Livepasses.Sdk;
using Livepasses.Sdk.Models;

var client = new LivepassesClient("lp_live_your_api_key");

// Generate a pass
var result = await client.Passes.GenerateAsync(new GeneratePassesParams
{
    TemplateId = "your-template-id",
    Passes =
    [
        new PassRecipient
        {
            Customer = new CustomerInfo { FirstName = "John", LastName = "Doe", Email = "john@example.com" },
            BusinessData = new BusinessData { SectionInfo = "A", RowInfo = "12", SeatNumber = "5" }
        }
    ]
});

Console.WriteLine($"Generated {result.TotalPasses} pass(es)");
Console.WriteLine($"Apple Wallet: {result.Passes[0].Platforms.Apple.AddToWalletUrl}");
Console.WriteLine($"Google Wallet: {result.Passes[0].Platforms.Google.AddToWalletUrl}");
```

## Configuration

```csharp
var client = new LivepassesClient("lp_live_your_api_key", new LivepassesOptions
{
    BaseUrl = "https://api.livepasses.com",  // default
    Timeout = TimeSpan.FromSeconds(30),       // default
    MaxRetries = 3                            // default
});
```

## Pass Types

### Event Passes

```csharp
var result = await client.Passes.GenerateAsync(new GeneratePassesParams
{
    TemplateId = "event-template-id",
    Passes =
    [
        new PassRecipient
        {
            Customer = new CustomerInfo { FirstName = "Jane", LastName = "Smith" },
            BusinessData = new BusinessData
            {
                SectionInfo = "VIP",
                RowInfo = "1",
                SeatNumber = "15",
                GateInfo = "Gate A",
                TicketType = "VIP",
                Price = 150.00m,
                Currency = "USD"
            }
        }
    ],
    BusinessContext = new BusinessContext
    {
        Event = new EventContext
        {
            EventName = "Tech Conference 2026",
            EventDate = "2026-06-15T09:00:00Z",
            DoorsOpen = "2026-06-15T08:00:00Z"
        }
    }
});
```

### Loyalty Cards

```csharp
var result = await client.Passes.GenerateAsync(new GeneratePassesParams
{
    TemplateId = "loyalty-template-id",
    Passes =
    [
        new PassRecipient
        {
            Customer = new CustomerInfo { FirstName = "John", LastName = "Doe" },
            BusinessData = new BusinessData
            {
                MembershipNumber = "MEM-001",
                CurrentPoints = 1500,
                MemberTier = "Gold",
                AccountBalance = 25.00m
            }
        }
    ]
});

// Earn loyalty points
await client.Passes.LoyaltyTransactAsync("pass-id", new LoyaltyTransactionParams
{
    TransactionType = "earn",
    Points = 500,
    Description = "Purchase reward"
});

// Spend loyalty points
await client.Passes.LoyaltyTransactAsync("pass-id", new LoyaltyTransactionParams
{
    TransactionType = "spend",
    Points = 100,
    Description = "Redeemed: Free coffee"
});
```

### Coupons

```csharp
var result = await client.Passes.GenerateAsync(new GeneratePassesParams
{
    TemplateId = "coupon-template-id",
    Passes =
    [
        new PassRecipient
        {
            Customer = new CustomerInfo { FirstName = "Jane", LastName = "Smith" },
            BusinessData = new BusinessData
            {
                PromoCode = "SAVE20",
                CampaignId = "summer-2026"
            }
        }
    ],
    BusinessContext = new BusinessContext
    {
        Coupon = new CouponContext
        {
            CampaignName = "Summer Sale",
            SpecialMessage = "20% off everything!",
            PromotionStartDate = "2026-06-01",
            PromotionEndDate = "2026-08-31"
        }
    }
});
```

## Async Batch Generation

For large batches, the API processes asynchronously. Use `GenerateAndWaitAsync` to handle polling automatically:

```csharp
var result = await client.Passes.GenerateAndWaitAsync(
    new GeneratePassesParams
    {
        TemplateId = "template-id",
        Passes = recipients // large list
    },
    new GenerateAndWaitOptions
    {
        PollInterval = 2000,  // ms between polls
        MaxAttempts = 150,
        OnProgress = status => Console.WriteLine($"Progress: {status.ProgressPercentage}%")
    });
```

### Batch Status (Manual Polling)

If you need more control over polling, use `GenerateAsync` + `GetBatchStatusAsync`:

```csharp
var initial = await client.Passes.GenerateAsync(new GeneratePassesParams
{
    TemplateId = "template-id",
    Passes = recipients
});

if (initial.BatchOperation != null)
{
    var batchId = initial.BatchOperation.BatchId;
    var status = await client.Passes.GetBatchStatusAsync(batchId);
    while (!status.IsCompleted)
    {
        await Task.Delay(2000);
        status = await client.Passes.GetBatchStatusAsync(batchId);
        Console.WriteLine($"Progress: {status.ProgressPercentage}%");
    }
    Console.WriteLine($"Completed: {status.Statistics.Successful} successful, {status.Statistics.Failed} failed");
}
```

## Pagination

### Manual pagination

```csharp
var page1 = await client.Passes.ListAsync(new ListPassesParams
{
    Page = 1,
    PageSize = 50,
    Status = "Active"
});
Console.WriteLine($"Page 1 of {page1.Pagination.TotalPages} ({page1.Pagination.TotalItems} total)");

// Get next page
if (page1.Pagination.CurrentPage < page1.Pagination.TotalPages)
{
    var page2 = await client.Passes.ListAsync(new ListPassesParams { Page = 2, PageSize = 50 });
}
```

### Auto-pagination

Use `await foreach` to lazily iterate through all pages:

```csharp
await foreach (var pass in client.Passes.ListAutoPaginateAsync(new ListPassesParams
{
    TemplateId = "template-id",
    Status = "Active"
}))
{
    Console.WriteLine($"{pass.Id} - {pass.HolderEmail}");
}
```

## Pass Operations

### Lookup

```csharp
// By pass ID
var pass = await client.Passes.LookupAsync(new LookupPassParams { PassId = "pass-id" });

// By pass number
var pass = await client.Passes.LookupAsync(new LookupPassParams { PassNumber = "LP-001" });
```

### Validate

```csharp
var validation = await client.Passes.ValidateAsync("pass-id");
if (validation.CanBeRedeemed)
{
    // proceed with redemption
}
```

### Redeem

```csharp
// Generic redemption
var redemption = await client.Passes.RedeemAsync("pass-id");

// Redemption with location and notes
var redemption = await client.Passes.RedeemAsync("pass-id", new RedeemPassParams
{
    Location = new RedemptionLocation { Name = "Store #1", Latitude = 4.6097, Longitude = -74.0817 },
    Notes = "Walk-in customer"
});
```

### Check-in (Events)

```csharp
await client.Passes.CheckInAsync("pass-id", new CheckInParams
{
    Location = new RedemptionLocation { Name = "Gate A", Latitude = 4.6097, Longitude = -74.0817 }
});
```

### Redeem Coupon

```csharp
await client.Passes.RedeemCouponAsync("pass-id", new RedeemCouponParams
{
    Location = new RedemptionLocation { Name = "Store #42" },
    Notes = "Applied to order #12345"
});
```

### Update a Pass

Update business data or context on an existing pass:

```csharp
await client.Passes.UpdateAsync("pass-id", new UpdatePassParams
{
    BusinessData = new BusinessData { CurrentPoints = 750, MemberTier = "Platinum" },
    BusinessContext = new BusinessContext
    {
        Loyalty = new LoyaltyContext
        {
            ProgramUpdate = "Congratulations on reaching Platinum!"
        }
    }
});
```

### Push a scoped update

Push field updates to all eligible passes of a template:

```csharp
await client.Passes.PushTemplateAsync("template-id", new PushTemplatePassesParams
{
    UpdatedFields = new Dictionary<string, object> { ["gate"] = "Gate C" },
    Reason = "Event-wide gate change"
});
```

## Template Management

### List and get

```csharp
// List templates
var templates = await client.Templates.ListAsync(new ListTemplatesParams
{
    Type = "Event",
    Status = "Active"
});

// Get template details
var template = await client.Templates.GetAsync("template-id");
```

### Create a template

```csharp
var template = await client.Templates.CreateAsync(new CreateTemplateParams
{
    Name = "VIP Event Pass",
    Description = "Premium event ticket template",
    BusinessFeatures = new Dictionary<string, object>
    {
        ["passType"] = "event",
        ["hasSeating"] = true,
        ["supportedPlatforms"] = new[] { "apple", "google" }
    }
});
Console.WriteLine($"Created: {template.Id} — {template.Name}");
```

### Update a template

```csharp
var updated = await client.Templates.UpdateAsync("template-id", new UpdateTemplateParams
{
    Name = "VIP Event Pass v2",
    Description = "Updated premium event ticket template"
});
```

### Activate / deactivate

```csharp
await client.Templates.ActivateAsync("template-id");
await client.Templates.DeactivateAsync("template-id");
```

## Webhooks

```csharp
// Create a webhook
var webhook = await client.Webhooks.CreateAsync(new CreateWebhookParams
{
    Url = "https://your-server.com/webhooks/livepasses",
    Events = [WebhookEventType.PassGenerated, WebhookEventType.PassRedeemed]
});
Console.WriteLine($"Secret: {webhook.Secret}"); // use to verify webhook signatures

// List webhooks
var webhooks = await client.Webhooks.ListAsync();

// Delete a webhook
await client.Webhooks.DeleteAsync("webhook-id");
```

## Error Handling

All API errors throw typed exceptions:

```csharp
using Livepasses.Sdk.Exceptions;

try
{
    await client.Passes.GenerateAsync(params);
}
catch (AuthenticationException)
{
    Console.WriteLine("Invalid API key");
}
catch (ValidationException ex)
{
    Console.WriteLine($"Validation error: {ex.Message} (code: {ex.Code})");
}
catch (ForbiddenException)
{
    Console.WriteLine("Insufficient permissions for this operation");
}
catch (RateLimitException ex)
{
    Console.WriteLine($"Rate limited. Retry after: {ex.RetryAfter}s");
}
catch (QuotaExceededException)
{
    Console.WriteLine("Monthly pass quota exceeded — upgrade your plan");
}
catch (NotFoundException)
{
    Console.WriteLine("Template or pass not found");
}
catch (BusinessRuleException ex)
{
    Console.WriteLine($"Business rule violation: {ex.Message}");
}
catch (LivepassesException ex)
{
    // Catch-all for any other API error
    Console.WriteLine($"API error [{ex.Code}]: {ex.Message} (HTTP {ex.Status})");
}
```

### Error codes

Use `ApiErrorCodes` for programmatic error code comparisons:

```csharp
using Livepasses.Sdk.Models;

try
{
    await client.Passes.RedeemAsync("pass-id");
}
catch (LivepassesException ex)
{
    var message = ex.Code switch
    {
        ApiErrorCodes.PassAlreadyUsed => "This pass has already been redeemed",
        ApiErrorCodes.PassExpired => "This pass has expired",
        ApiErrorCodes.TemplateInactive => "The template for this pass is inactive",
        _ => $"Unhandled error: {ex.Code}"
    };
    Console.WriteLine(message);
}
```

### Exception hierarchy

| Exception | HTTP Status | When |
|-----------|------------|------|
| `AuthenticationException` | 401 | Invalid, expired, or revoked API key |
| `ValidationException` | 400 | Request validation failed |
| `ForbiddenException` | 403 | Insufficient permissions |
| `NotFoundException` | 404 | Resource not found |
| `RateLimitException` | 429 | Rate limit exceeded |
| `QuotaExceededException` | 403 | API quota or subscription limit exceeded |
| `BusinessRuleException` | 422 | Business rule violation (pass expired, already used, etc.) |

### Automatic retries

The SDK automatically retries on:
- **429 Too Many Requests** — honors `Retry-After` header
- **5xx Server Errors** — exponential backoff with jitter

## CancellationToken Support

Async methods that perform polling accept a `CancellationToken`:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

var result = await client.Passes.GenerateAndWaitAsync(
    params,
    new GenerateAndWaitOptions { PollInterval = 2000 },
    cts.Token);

await foreach (var pass in client.Passes.ListAutoPaginateAsync(
    new ListPassesParams { TemplateId = "tpl-id" },
    cts.Token))
{
    Console.WriteLine(pass.Id);
}
```

## Examples

See the [`examples/`](./examples/) directory for runnable projects:

- **[GeneratePasses](./examples/GeneratePasses/)** — End-to-end pass generation, lookup, validation, and check-in
- **[LoyaltyWorkflow](./examples/LoyaltyWorkflow/)** — Loyalty card lifecycle: generate, earn points, spend points, update tier
- **[CouponWorkflow](./examples/CouponWorkflow/)** — Coupon pass generation and redemption
- **[TemplateManagement](./examples/TemplateManagement/)** — CRUD operations on pass templates
- **[WebhookSetup](./examples/WebhookSetup/)** — Register, list, and manage webhooks

Run any example with:
```bash
export LIVEPASSES_API_KEY="your-api-key"
cd examples/GeneratePasses
dotnet run
```

## License

MIT
