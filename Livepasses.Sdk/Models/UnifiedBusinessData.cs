namespace Livepasses.Sdk.Models;

/// <summary>
/// Unified business data returned with a generated pass.
/// </summary>
public record UnifiedBusinessData
{
    public string? Section { get; init; }
    public string? Row { get; init; }
    public string? Seat { get; init; }
    public string? Gate { get; init; }
    public string? TicketType { get; init; }
    public string? FormattedPrice { get; init; }
    public string? MembershipNumber { get; init; }
    public int? CurrentPoints { get; init; }
    public string? MemberTier { get; init; }
    public string? FormattedBalance { get; init; }
    public string? PromoCode { get; init; }
    public string? DiscountDescription { get; init; }
    public string? ValidityDescription { get; init; }
}
