namespace Livepasses.Sdk.Models;

/// <summary>
/// A template in the template list.
/// </summary>
public record TemplateListItem
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public string Type { get; init; } = default!;
    public string Status { get; init; } = default!;
    public int PassCount { get; init; }
    public string CreatedAt { get; init; } = default!;
    public string? UpdatedAt { get; init; }
}
