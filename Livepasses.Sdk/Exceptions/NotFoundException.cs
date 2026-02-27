namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when the requested resource is not found (HTTP 404).
/// </summary>
public class NotFoundException : LivepassesException
{
    /// <summary>
    /// Creates a new <see cref="NotFoundException"/>.
    /// </summary>
    public NotFoundException(string message, string code, string? details = null)
        : base(message, 404, code, details) { }
}
