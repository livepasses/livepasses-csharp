namespace Livepasses.Sdk.Exceptions;

/// <summary>
/// Base exception for all Livepasses API errors.
/// </summary>
public class LivepassesException : Exception
{
    /// <summary>HTTP status code returned by the API.</summary>
    public int Status { get; }

    /// <summary>Machine-readable error code from the API.</summary>
    public string Code { get; }

    /// <summary>Additional error details, if available.</summary>
    public string? Details { get; }

    /// <summary>
    /// Creates a new <see cref="LivepassesException"/>.
    /// </summary>
    public LivepassesException(string message, int status, string code, string? details = null)
        : base(message)
    {
        Status = status;
        Code = code;
        Details = details;
    }
}
