namespace Livepasses.Sdk.Models;

/// <summary>
/// Coupon-specific business context.
/// </summary>
public class CouponContext
{
    public string? CampaignName { get; set; }
    public string? SpecialMessage { get; set; }
    public string? PromotionStartDate { get; set; }
    public string? PromotionEndDate { get; set; }
}
