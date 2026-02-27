namespace Livepasses.Sdk.Models;

/// <summary>
/// Result of a pass generation request.
/// </summary>
public record PassGenerationResult
{
    public string BatchId { get; init; } = default!;
    public string TemplateId { get; init; } = default!;
    public string GeneratedAt { get; init; } = default!;
    public int TotalPasses { get; init; }
    public bool IsAsyncProcessing { get; init; }
    public BatchOperationInfo? BatchOperation { get; init; }
    public List<GeneratedPass> Passes { get; init; } = [];
    public PassDeliveryResult? Delivery { get; init; }
    public BusinessMetrics? BusinessMetrics { get; init; }
}
