namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for generating passes.
/// </summary>
public class GeneratePassesParams
{
    public required string TemplateId { get; set; }
    public required List<PassRecipient> Passes { get; set; }
    public BusinessContext? BusinessContext { get; set; }
    public PassGenerationOptions? Options { get; set; }
}
