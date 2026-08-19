namespace Livepasses.Sdk.Models;

/// <summary>
/// Events the API accepts on a webhook subscription.
///
/// Mirrors the server's allow-list exactly. Subscribing to anything outside it is rejected with a
/// 400, so a value that is not here is not a "not yet supported" event — it is a request that
/// always fails.
/// </summary>
public static class WebhookEventType
{
    public const string PassGenerated = "pass.generated";
    public const string PassRedeemed = "pass.redeemed";
    public const string PassUpdated = "pass.updated";

    // Loyalty and coupon activity
    public const string LoyaltyTransacted = "loyalty.transacted";
    public const string CouponApplied = "coupon.applied";

    // Transfer lifecycle events
    public const string TransferInitiated = "transfer.initiated";
    public const string TransferAccepted = "transfer.accepted";
    public const string TransferDeclined = "transfer.declined";
    public const string TransferRevoked = "transfer.revoked";
    public const string TransferExpired = "transfer.expired";

    /// <summary>Every event above.</summary>
    public const string All = "*";
}
