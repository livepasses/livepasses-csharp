namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for updating a pass.
/// </summary>
public class UpdatePassParams
{
    public BusinessData? BusinessData { get; set; }
    public BusinessContext? BusinessContext { get; set; }
}
