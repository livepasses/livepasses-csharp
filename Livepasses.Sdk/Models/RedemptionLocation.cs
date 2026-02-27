namespace Livepasses.Sdk.Models;

/// <summary>
/// Location data for pass redemption or check-in.
/// </summary>
public class RedemptionLocation
{
    public string? Name { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
