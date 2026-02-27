namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when the request fails validation (HTTP 400).
/// </summary>
public class ValidationException : LivepassesException
{
    /// <summary>
    /// Creates a new <see cref="ValidationException"/>.
    /// </summary>
    public ValidationException(string message, string code, string? details = null)
        : base(message, 400, code, details) { }
}
