namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for updating a template.
/// </summary>
public class UpdateTemplateParams
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, object>? BusinessFeatures { get; set; }
}
