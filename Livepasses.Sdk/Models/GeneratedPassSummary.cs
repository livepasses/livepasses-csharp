namespace Livepasses.Sdk.Models;

/// <summary>
/// Summary of a generated pass within a batch result.
/// </summary>
public record GeneratedPassSummary
{
    public string Id { get; init; } = default!;
    public string PassNumber { get; init; } = default!;
    public string? HolderEmail { get; init; }
    public string? HolderName { get; init; }
    public string Status { get; init; } = default!;
    public bool HasApplePass { get; init; }
    public bool HasGooglePass { get; init; }
    public string GeneratedAt { get; init; } = default!;
}
