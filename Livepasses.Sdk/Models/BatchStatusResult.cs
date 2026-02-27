namespace Livepasses.Sdk.Models;

/// <summary>
/// Current status of a batch pass generation operation.
/// </summary>
public record BatchStatusResult
{
    public string Id { get; init; } = default!;
    public string Status { get; init; } = default!;
    public int TotalRecipients { get; init; }
    public int PassesGenerated { get; init; }
    public int PassesDelivered { get; init; }
    public int GenerationFailures { get; init; }
    public int DeliveryFailures { get; init; }
    public double ProgressPercentage { get; init; }
    public string? StartedAt { get; init; }
    public string? CompletedAt { get; init; }
    public string? EstimatedCompletion { get; init; }
    public string? ErrorMessage { get; init; }
    public bool IsCompleted { get; init; }
    public bool IsActive { get; init; }
    public BatchStatistics? Statistics { get; init; }
    public List<GeneratedPassSummary>? GeneratedPasses { get; init; }
}
