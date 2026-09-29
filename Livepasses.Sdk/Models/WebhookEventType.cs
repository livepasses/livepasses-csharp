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
    /// <summary>The holder saved the pass to a wallet: it went from no device to one. A second
    /// device does not fire it again; a re-add after PassRemoved does.</summary>
    public const string PassInstalled = "pass.installed";
    /// <summary>The holder removed the pass from their wallet and it is on no device. Holder-
    /// initiated only — cancellation, transfer and operator ejection never fire it.</summary>
    public const string PassRemoved = "pass.removed";
    public const string PassRedeemed = "pass.redeemed";
    public const string PassUpdated = "pass.updated";
    public const string PassCancelled = "pass.cancelled";
    public const string PassExpired = "pass.expired";

    // Loyalty and coupon activity
    public const string LoyaltyTransacted = "loyalty.transacted";
    public const string CouponApplied = "coupon.applied";

    /// <summary>A membership pass was scanned at a door. Distinct from PassRedeemed, which for a
    /// single-use pass means the entitlement is now spent.</summary>
    public const string MembershipCheckedIn = "membership.checked_in";

    // Transfer lifecycle events
    public const string TransferInitiated = "transfer.initiated";
    public const string TransferAccepted = "transfer.accepted";
    public const string TransferDeclined = "transfer.declined";
    public const string TransferRevoked = "transfer.revoked";
    public const string TransferExpired = "transfer.expired";

    // Fraud detection
    /// <summary>
    /// Advisory: raised when membership sharing detection flags a pass, e.g. the same card
    /// checking in at too many distinct venues within a window. The triggering check-in still
    /// succeeded; this event never blocks or denies anything.
    /// </summary>
    public const string PassSharingSuspected = "pass.sharing_suspected";

    /// <summary>Every event above.</summary>
    public const string All = "*";
}
