namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for redeeming a coupon pass.
/// </summary>
public class RedeemCouponParams
{
    public RedemptionLocation? Location { get; set; }

    /// <summary>
    /// Free-form key/value pairs recorded with the redemption. Use it for anything you want kept
    /// alongside the redemption, such as an order number or a staff note.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}
