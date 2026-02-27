namespace Livepasses.Sdk.Models;

/// <summary>
/// Result of a pass lookup.
/// </summary>
public record PassLookupResult
{
    public string PassId { get; init; } = default!;
    public string PassNumber { get; init; } = default!;
    public string TemplateId { get; init; } = default!;
    public string TemplateName { get; init; } = default!;
    public string TemplateType { get; init; } = default!;
    public string? HolderName { get; init; }
    public string? HolderEmail { get; init; }
    public string Status { get; init; } = default!;
    public bool IsValid { get; init; }
    public bool CanBeRedeemed { get; init; }
    public bool IsExpired { get; init; }
    public string? ValidFrom { get; init; }
    public string? ValidUntil { get; init; }
    public string? RedeemedAt { get; init; }
    public string GeneratedAt { get; init; } = default!;
}
