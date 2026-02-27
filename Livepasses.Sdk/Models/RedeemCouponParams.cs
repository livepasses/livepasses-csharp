namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for redeeming a coupon pass.
/// </summary>
public class RedeemCouponParams
{
    public RedemptionLocation? Location { get; set; }
    public string? Notes { get; set; }
}
