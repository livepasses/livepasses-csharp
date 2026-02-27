namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when the API key is invalid, expired, or revoked (HTTP 401).
/// </summary>
public class AuthenticationException : LivepassesException
{
    /// <summary>
    /// Creates a new <see cref="AuthenticationException"/>.
    /// </summary>
    public AuthenticationException(string message, string code, string? details = null)
        : base(message, 401, code, details) { }
}
