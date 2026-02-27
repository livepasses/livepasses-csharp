namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for checking in an event pass.
/// </summary>
public class CheckInParams
{
    public RedemptionLocation? Location { get; set; }
    public string? Notes { get; set; }
}
