namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when an API quota or subscription limit is exceeded. The API answers 422;
/// <see cref="LivepassesException.Status"/> carries the real status.
/// </summary>
public class QuotaExceededException : LivepassesException
{
    /// <summary>
    /// Creates a new <see cref="QuotaExceededException"/> with the historical status, 403.
    /// </summary>
    public QuotaExceededException(string message, string code, string? details = null)
        : this(message, code, details, 403) { }

    /// <summary>
    /// Creates a new <see cref="QuotaExceededException"/> carrying the response's real HTTP status.
    /// </summary>
    public QuotaExceededException(string message, string code, string? details, int status)
        : base(message, status, code, details) { }
}
