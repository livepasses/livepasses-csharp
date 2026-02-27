namespace Livepasses.Sdk.Models;

/// <summary>
/// A single pass recipient with customer info and business data.
/// </summary>
public class PassRecipient
{
    public required CustomerInfo Customer { get; set; }
    public required BusinessData BusinessData { get; set; }
    public PersonalizationData? Personalizations { get; set; }
}
