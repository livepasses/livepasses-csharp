namespace Livepasses.Sdk.Models;

/// <summary>
/// Shared business context for pass generation (event, loyalty, coupon).
/// </summary>
public class BusinessContext
{
    public EventContext? Event { get; set; }
    public LoyaltyContext? Loyalty { get; set; }
    public CouponContext? Coupon { get; set; }
}
