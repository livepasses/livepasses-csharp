namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for redeeming a pass.
/// </summary>
public class RedeemPassParams
{
    public RedemptionLocation? Location { get; set; }
    public string? Notes { get; set; }
}
