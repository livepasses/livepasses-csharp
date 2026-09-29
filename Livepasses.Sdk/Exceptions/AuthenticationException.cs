namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when the API key is invalid, expired, or revoked (HTTP 401).
/// </summary>
public class AuthenticationException : LivepassesException
{
    /// <summary>
    /// Creates a new <see cref="AuthenticationException"/> with the historical status, 401.
    /// </summary>
    public AuthenticationException(string message, string code, string? details = null)
        : this(message, code, details, 401) { }

    /// <summary>
    /// Creates a new <see cref="AuthenticationException"/> carrying the response's real HTTP status.
    /// </summary>
    public AuthenticationException(string message, string code, string? details, int status)
        : base(message, status, code, details) { }
}
