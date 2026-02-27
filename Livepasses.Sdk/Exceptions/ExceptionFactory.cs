namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Maps API error codes and HTTP status codes to typed exceptions.
/// </summary>
internal static class ExceptionFactory
{
    private static readonly HashSet<string> AuthCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "UNAUTHORIZED", "INVALID_API_KEY", "API_KEY_EXPIRED", "API_KEY_REVOKED"
    };

    private static readonly HashSet<string> ForbiddenCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "FORBIDDEN", "INSUFFICIENT_PERMISSIONS"
    };

    private static readonly HashSet<string> ValidationCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "VALIDATION_ERROR", "REQUIRED_FIELD_MISSING", "INVALID_FIELD_VALUE",
        "INVALID_FIELD_FORMAT", "FIELD_TOO_LONG", "FIELD_TOO_SHORT"
    };

    private static readonly HashSet<string> NotFoundCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "NOT_FOUND", "PASS_NOT_FOUND", "TEMPLATE_NOT_FOUND",
        "TENANT_NOT_FOUND", "PARTNERSHIP_NOT_FOUND"
    };

    private static readonly HashSet<string> RateLimitCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "RATE_LIMIT_EXCEEDED", "TOO_MANY_REQUESTS"
    };

    private static readonly HashSet<string> QuotaCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "QUOTA_EXCEEDED", "API_QUOTA_EXCEEDED", "SUBSCRIPTION_REQUIRED", "FEATURE_NOT_AVAILABLE"
    };

    private static readonly HashSet<string> BusinessRuleCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "BUSINESS_RULE_VIOLATION", "OPERATION_NOT_ALLOWED", "PASS_EXPIRED",
        "PASS_ALREADY_USED", "TEMPLATE_INACTIVE", "RESOURCE_LOCKED", "RESOURCE_EXPIRED"
    };

    /// <summary>
    /// Creates a typed exception from the API error response.
    /// </summary>
    public static LivepassesException Create(
        string message, int status, string code, string? details = null, int? retryAfter = null)
    {
        // Match by error code first
        if (AuthCodes.Contains(code))
            return new AuthenticationException(message, code, details);

        if (ForbiddenCodes.Contains(code))
            return new ForbiddenException(message, code, details);

        if (ValidationCodes.Contains(code))
            return new ValidationException(message, code, details);

        if (NotFoundCodes.Contains(code))
            return new NotFoundException(message, code, details);

        if (RateLimitCodes.Contains(code))
            return new RateLimitException(message, code, details, retryAfter);

        if (QuotaCodes.Contains(code))
            return new QuotaExceededException(message, code, details);

        if (BusinessRuleCodes.Contains(code))
            return new BusinessRuleException(message, code, details);

        // Fall back to HTTP status
        return status switch
        {
            401 => new AuthenticationException(message, code, details),
            400 => new ValidationException(message, code, details),
            403 => new ForbiddenException(message, code, details),
            404 => new NotFoundException(message, code, details),
            429 => new RateLimitException(message, code, details, retryAfter),
            422 => new BusinessRuleException(message, code, details),
            _ => new LivepassesException(message, status, code, details)
        };
    }
}
