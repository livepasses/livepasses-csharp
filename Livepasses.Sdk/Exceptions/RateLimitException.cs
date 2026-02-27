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
        : base(message, 429, code, details)
    {
        RetryAfter = retryAfter;
    }
}
