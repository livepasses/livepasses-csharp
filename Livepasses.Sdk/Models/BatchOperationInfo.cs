namespace Livepasses.Sdk.Models;

/// <summary>
/// Batch operation status information.
/// </summary>
public record BatchOperationInfo
{
    public string Id { get; init; } = default!;
    public string Status { get; init; } = default!;
    public int TotalRecipients { get; init; }
    public int PassesGenerated { get; init; }
    public int GenerationFailures { get; init; }
    public double ProgressPercentage { get; init; }
    public string? EstimatedCompletion { get; init; }
    public string StatusPollUrl { get; init; } = default!;
}
