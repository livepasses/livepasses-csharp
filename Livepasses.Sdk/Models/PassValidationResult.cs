namespace Livepasses.Sdk.Models;

/// <summary>
/// Result of validating a pass.
/// </summary>
public record PassValidationResult
{
    public string PassId { get; init; } = default!;
    public string PassNumber { get; init; } = default!;
    public string Status { get; init; } = default!;
    public bool CanBeRedeemed { get; init; }
    public bool IsExpired { get; init; }
    public string ValidationMessage { get; init; } = default!;
    public string TemplateType { get; init; } = default!;
    public string? HolderName { get; init; }
    public string? HolderEmail { get; init; }
    public string? ValidFrom { get; init; }
    public string? ValidUntil { get; init; }
    public List<string> VerificationMethods { get; init; } = [];
}
