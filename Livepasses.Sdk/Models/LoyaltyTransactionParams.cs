namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for a loyalty points transaction.
/// </summary>
public class LoyaltyTransactionParams
{
    public required string TransactionType { get; set; }
    public required int Points { get; set; }
    public string? Description { get; set; }
}
