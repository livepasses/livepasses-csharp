namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for deducting an amount from a gift card's balance.
///
/// A redemption for more than the remaining balance is rejected, and the balance is left
/// untouched in that case.
/// </summary>
public class RedeemGiftCardParams
{
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public string? RedemptionChannel { get; set; }
    public RedemptionLocation? Location { get; set; }
}
