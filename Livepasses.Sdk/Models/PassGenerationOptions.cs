namespace Livepasses.Sdk.Models;

/// <summary>
/// Options for pass generation.
/// </summary>
public class PassGenerationOptions
{
    public string? DeliveryMethod { get; set; }
    public bool? GenerateForAllPlatforms { get; set; }
}
