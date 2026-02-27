namespace Livepasses.Sdk.Models;

/// <summary>
/// Known webhook event types.
/// </summary>
public static class WebhookEventType
{
    public const string PassGenerated = "pass.generated";
    public const string PassRedeemed = "pass.redeemed";
    public const string PassUpdated = "pass.updated";
    public const string PassExpired = "pass.expired";
    public const string PassCheckedIn = "pass.checked_in";
    public const string BatchCompleted = "batch.completed";
    public const string BatchFailed = "batch.failed";
}
