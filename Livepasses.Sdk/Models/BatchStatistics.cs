namespace Livepasses.Sdk.Models;

/// <summary>
/// Statistics for a completed batch operation.
/// </summary>
public record BatchStatistics
{
    public double AverageGenerationTimeSeconds { get; init; }
    public double GenerationSuccessRate { get; init; }
    public double DeliverySuccessRate { get; init; }
    public string? MostCommonError { get; init; }
    public string? TotalDuration { get; init; }
}
