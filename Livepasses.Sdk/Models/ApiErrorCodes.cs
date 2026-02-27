namespace Livepasses.Sdk.Models;

/// <summary>
/// All known API error codes.
/// </summary>
public static class ApiErrorCodes
{
    // General
    public const string InternalError = "INTERNAL_ERROR";
    public const string ServiceUnavailable = "SERVICE_UNAVAILABLE";
    public const string Timeout = "TIMEOUT";
    public const string NetworkError = "NETWORK_ERROR";

    // Authentication & Authorization
    public const string Unauthorized = "UNAUTHORIZED";
    public const string InvalidApiKey = "INVALID_API_KEY";
    public const string ApiKeyExpired = "API_KEY_EXPIRED";
    public const string ApiKeyRevoked = "API_KEY_REVOKED";
    public const string Forbidden = "FORBIDDEN";
    public const string InsufficientPermissions = "INSUFFICIENT_PERMISSIONS";

    // Validation
    public const string ValidationError = "VALIDATION_ERROR";
    public const string RequiredFieldMissing = "REQUIRED_FIELD_MISSING";
    public const string InvalidFieldValue = "INVALID_FIELD_VALUE";
    public const string InvalidFieldFormat = "INVALID_FIELD_FORMAT";
    public const string FieldTooLong = "FIELD_TOO_LONG";
    public const string FieldTooShort = "FIELD_TOO_SHORT";

    // Resource
    public const string NotFound = "NOT_FOUND";
    public const string PassNotFound = "PASS_NOT_FOUND";
    public const string TemplateNotFound = "TEMPLATE_NOT_FOUND";
    public const string TenantNotFound = "TENANT_NOT_FOUND";
    public const string PartnershipNotFound = "PARTNERSHIP_NOT_FOUND";

    // Business Rules
    public const string BusinessRuleViolation = "BUSINESS_RULE_VIOLATION";
    public const string OperationNotAllowed = "OPERATION_NOT_ALLOWED";
    public const string PassExpired = "PASS_EXPIRED";
    public const string PassAlreadyUsed = "PASS_ALREADY_USED";
    public const string TemplateInactive = "TEMPLATE_INACTIVE";
    public const string ResourceLocked = "RESOURCE_LOCKED";
    public const string ResourceExpired = "RESOURCE_EXPIRED";

    // Subscription & Quota
    public const string QuotaExceeded = "QUOTA_EXCEEDED";
    public const string ApiQuotaExceeded = "API_QUOTA_EXCEEDED";
    public const string SubscriptionRequired = "SUBSCRIPTION_REQUIRED";
    public const string FeatureNotAvailable = "FEATURE_NOT_AVAILABLE";

    // Rate Limiting
    public const string RateLimitExceeded = "RATE_LIMIT_EXCEEDED";
    public const string TooManyRequests = "TOO_MANY_REQUESTS";
}
