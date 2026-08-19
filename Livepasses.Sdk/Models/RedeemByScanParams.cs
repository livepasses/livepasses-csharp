namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for resolving a scanned barcode or NFC tap value and redeeming it in one call, so a
/// scanner does not need a separate lookup round-trip first.
/// </summary>
public class RedeemByScanParams
{
    public string ScannedValue { get; set; } = string.Empty;
    public string? RedemptionMethod { get; set; }
    public string? RedemptionChannel { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
