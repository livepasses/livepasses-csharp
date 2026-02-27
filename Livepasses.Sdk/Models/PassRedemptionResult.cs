namespace Livepasses.Sdk.Models;

/// <summary>
/// Result of redeeming, checking in, or transacting on a pass.
/// </summary>
public record PassRedemptionResult
{
    public string PassId { get; init; } = default!;
    public string PassNumber { get; init; } = default!;
    public string RedeemedAt { get; init; } = default!;
    public string RedemptionMethod { get; init; } = default!;
    public string PreviousStatus { get; init; } = default!;
    public string NewStatus { get; init; } = default!;
    public bool AlreadyRedeemed { get; init; }
}
