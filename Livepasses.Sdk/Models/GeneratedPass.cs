namespace Livepasses.Sdk.Models;

/// <summary>
/// A generated pass with platform URLs and business data.
/// </summary>
public record GeneratedPass
{
    public string Id { get; init; } = default!;
    public string? CustomerEmail { get; init; }
    public string? ConfirmationCode { get; init; }
    public PassPlatforms Platforms { get; init; } = new();
    public UnifiedBusinessData BusinessData { get; init; } = new();
    public string? QrCode { get; init; }
    public string Status { get; init; } = default!;

    /// <summary>Present when <see cref="Status"/> is "failed", e.g. "MEMBERSHIP_NUMBER_CONFLICT".</summary>
    public string? ErrorCode { get; init; }

    /// <summary>Present when <see cref="Status"/> is "failed".</summary>
    public string? ErrorMessage { get; init; }

    public AnalyticsInfo? Analytics { get; init; }
}
