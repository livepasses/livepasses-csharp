namespace Livepasses.Sdk.Models;

/// <summary>
/// Customer information for pass generation.
/// </summary>
public class CustomerInfo
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PreferredLanguage { get; set; }
}
