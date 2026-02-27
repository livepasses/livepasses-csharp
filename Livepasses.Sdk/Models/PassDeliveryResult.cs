namespace Livepasses.Sdk.Models;

/// <summary>
/// Pass delivery result with method, status, and per-recipient details.
/// </summary>
public record PassDeliveryResult
{
    public string Method { get; init; } = default!;
    public string Status { get; init; } = default!;
    public string? SentAt { get; init; }
    public List<DeliveryDetail> Details { get; init; } = [];
}
