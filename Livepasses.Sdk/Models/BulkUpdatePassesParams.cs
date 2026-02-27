namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for bulk-updating multiple passes.
/// </summary>
public class BulkUpdatePassesParams
{
    public required List<string> PassIds { get; set; }
    public BusinessData? BusinessData { get; set; }
    public BusinessContext? BusinessContext { get; set; }
}
