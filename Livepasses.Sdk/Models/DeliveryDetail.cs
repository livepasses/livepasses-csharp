namespace Livepasses.Sdk.Models;

/// <summary>
/// Delivery detail for a single recipient.
/// </summary>
public record DeliveryDetail
{
    public string? Recipient { get; init; }
    public string Status { get; init; } = default!;
}
