namespace Livepasses.Sdk.Models;

/// <summary>
/// Detailed template information.
/// </summary>
public record TemplateDetail : TemplateListItem
{
    public Dictionary<string, object>? BusinessFeatures { get; init; }
    public Dictionary<string, object>? PlatformSupport { get; init; }
    public Dictionary<string, object>? MediaConfiguration { get; init; }
}
