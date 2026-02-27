namespace Livepasses.Sdk.Models;

/// <summary>
/// Analytics tracking information for a pass.
/// </summary>
public record AnalyticsInfo
{
    public string TrackingId { get; init; } = default!;
    public string EngagementUrl { get; init; } = default!;
}
