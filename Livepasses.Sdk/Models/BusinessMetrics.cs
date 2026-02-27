namespace Livepasses.Sdk.Models;

/// <summary>
/// Business metrics from pass generation.
/// </summary>
public record BusinessMetrics
{
    public decimal? TotalRevenue { get; init; }
    public decimal? AverageTicketPrice { get; init; }
    public Dictionary<string, int>? SeatDistribution { get; init; }
    public Dictionary<string, int>? TierDistribution { get; init; }
    public Dictionary<string, decimal>? RevenueByCategory { get; init; }
}
