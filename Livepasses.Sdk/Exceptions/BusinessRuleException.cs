namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when a business rule is violated (HTTP 422).
/// </summary>
public class BusinessRuleException : LivepassesException
{
    /// <summary>
    /// Creates a new <see cref="BusinessRuleException"/>.
    /// </summary>
    public BusinessRuleException(string message, string code, string? details = null)
        : base(message, 422, code, details) { }
}
