namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when the request is forbidden due to insufficient permissions (HTTP 403).
/// </summary>
public class ForbiddenException : LivepassesException
{
    /// <summary>
    /// Creates a new <see cref="ForbiddenException"/> with the historical status, 403.
    /// </summary>
    public ForbiddenException(string message, string code, string? details = null)
        : this(message, code, details, 403) { }

    /// <summary>
    /// Creates a new <see cref="ForbiddenException"/> carrying the response's real HTTP status.
    /// </summary>
    public ForbiddenException(string message, string code, string? details, int status)
        : base(message, status, code, details) { }
}
