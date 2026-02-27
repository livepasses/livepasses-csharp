namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for creating a new template.
/// </summary>
public class CreateTemplateParams
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required Dictionary<string, object> BusinessFeatures { get; set; }
    public List<string>? RequiredMedia { get; set; }
}
