namespace Livepasses.Sdk.Models;

/// <summary>
/// Business data for a pass recipient, covering event, loyalty, and coupon fields.
/// </summary>
public class BusinessData
{
    // Event fields
    public string? SectionInfo { get; set; }
    public string? RowInfo { get; set; }
    public string? SeatNumber { get; set; }
    public string? GateInfo { get; set; }
    public string? ConfirmationCode { get; set; }
    public string? TicketType { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }

    // Loyalty fields
    public string? MembershipNumber { get; set; }
    public int? CurrentPoints { get; set; }
    public string? MemberTier { get; set; }
    public int? LifetimePoints { get; set; }
    public decimal? AccountBalance { get; set; }
    public string? MemberSince { get; set; }
    public string? FavoriteStore { get; set; }

    // Coupon fields
    public string? PromoCode { get; set; }
    public string? CampaignId { get; set; }
    public string? CustomerSegment { get; set; }
    public int? MaxUsageCount { get; set; }
    public string? SourceChannel { get; set; }
}
