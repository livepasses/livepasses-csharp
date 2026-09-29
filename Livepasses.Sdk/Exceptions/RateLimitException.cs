namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when the API rate limit is exceeded (HTTP 429).
/// </summary>
public class RateLimitException : LivepassesException
{
    /// <summary>Number of seconds to wait before retrying, if provided by the API.</summary>
    public int? RetryAfter { get; }

    /// <summary>
    /// Creates a new <see cref="RateLimitException"/>.
    /// </summary>
    public RateLimitException(string message, string code, string? details = null, int? retryAfter = null)
        : this(message, code, details, retryAfter, 429) { }

    /// <summary>
    /// Creates a new <see cref="RateLimitException"/> carrying the response's real HTTP status.
    /// </summary>
    public RateLimitException(string message, string code, string? details, int? retryAfter, int status)
        : base(message, status, code, details)
    {
        RetryAfter = retryAfter;
    }
}
