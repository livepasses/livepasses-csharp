namespace Livepasses.Sdk.Models;

/// <summary>
/// A pass in the global pass list.
/// </summary>
public record GlobalPassDto
{
    public string Id { get; init; } = default!;
    public string SerialNumber { get; init; } = default!;
    public string Platform { get; init; } = default!;
    public string Status { get; init; } = default!;
    public string GeneratedAt { get; init; } = default!;
    public string? RedeemedAt { get; init; }
    public string? ValidUntil { get; init; }
    public string? HolderEmail { get; init; }
    public string TemplateId { get; init; } = default!;
    public string TemplateName { get; init; } = default!;
    public string TemplateType { get; init; } = default!;
}
