namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Thrown when the request fails validation (HTTP 400).
/// </summary>
public class ValidationException : LivepassesException
{
    /// <summary>
    /// Field-level validation failures, keyed by the API's camelCase field path (for example
    /// <c>operations[0].path</c>). Only present for VALIDATION_ERROR.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? Fields { get; }

    /// <summary>
    /// Creates a new <see cref="ValidationException"/> with the historical status, 400.
    /// </summary>
    public ValidationException(string message, string code, string? details = null, IReadOnlyDictionary<string, string[]>? fields = null)
        : this(message, code, details, fields, 400) { }

    /// <summary>
    /// Creates a new <see cref="ValidationException"/> carrying the response's real HTTP status.
    /// </summary>
    public ValidationException(string message, string code, string? details, IReadOnlyDictionary<string, string[]>? fields, int status)
        : base(message, status, code, details)
    {
        Fields = fields;
    }
}
