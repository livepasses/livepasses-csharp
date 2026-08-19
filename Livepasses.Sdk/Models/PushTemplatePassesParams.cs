namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for pushing a scoped update to all eligible passes of a template.
/// </summary>
public class PushTemplatePassesParams
{
    public required Dictionary<string, object> UpdatedFields { get; set; }
    public string? Reason { get; set; }
}
