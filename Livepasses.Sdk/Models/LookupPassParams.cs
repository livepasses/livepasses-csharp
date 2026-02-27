namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for looking up a pass by ID or number.
/// </summary>
public class LookupPassParams
{
    public string? PassId { get; set; }
    public string? PassNumber { get; set; }
}
