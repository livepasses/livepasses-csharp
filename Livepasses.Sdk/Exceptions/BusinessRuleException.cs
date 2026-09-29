namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when a business rule is violated (HTTP 422).
/// </summary>
public class BusinessRuleException : LivepassesException
{
    /// <summary>
    /// Creates a new <see cref="BusinessRuleException"/> with the historical status, 422.
    /// </summary>
    public BusinessRuleException(string message, string code, string? details = null)
        : this(message, code, details, 422) { }

    /// <summary>
    /// Creates a new <see cref="BusinessRuleException"/> carrying the response's real HTTP status.
    /// </summary>
    public BusinessRuleException(string message, string code, string? details, int status)
        : base(message, status, code, details) { }
}
