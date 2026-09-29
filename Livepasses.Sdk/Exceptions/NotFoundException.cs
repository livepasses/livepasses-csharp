namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when the requested resource is not found (HTTP 404).
/// </summary>
public class NotFoundException : LivepassesException
{
    /// <summary>
    /// Creates a new <see cref="NotFoundException"/> with the historical status, 404.
    /// </summary>
    public NotFoundException(string message, string code, string? details = null)
        : this(message, code, details, 404) { }

    /// <summary>
    /// Creates a new <see cref="NotFoundException"/> carrying the response's real HTTP status.
    /// </summary>
    public NotFoundException(string message, string code, string? details, int status)
        : base(message, status, code, details) { }
}
