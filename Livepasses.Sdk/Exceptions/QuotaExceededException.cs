namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when an API quota or subscription limit is exceeded (HTTP 403).
/// </summary>
public class QuotaExceededException : LivepassesException
{
    /// <summary>
    /// Creates a new <see cref="QuotaExceededException"/>.
    /// </summary>
    public QuotaExceededException(string message, string code, string? details = null)
        : base(message, 403, code, details) { }
}
