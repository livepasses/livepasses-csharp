namespace Livepasses.Sdk.Models;

/// <summary>
/// Personalization data for a pass recipient.
/// </summary>
public class PersonalizationData
{
    public string? DietaryRestrictions { get; set; }
    public string? AccessibilityNeeds { get; set; }
    public Dictionary<string, string>? CustomFields { get; set; }
}
