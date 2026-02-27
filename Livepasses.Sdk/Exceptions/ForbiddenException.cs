namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when the request is forbidden due to insufficient permissions (HTTP 403).
/// </summary>
public class ForbiddenException : LivepassesException
{
    /// <summary>
    /// Creates a new <see cref="ForbiddenException"/>.
    /// </summary>
    public ForbiddenException(string message, string code, string? details = null)
        : base(message, 403, code, details) { }
}
