namespace Livepasses.Sdk.Models;

/// <summary>
/// Standard API envelope response.
/// </summary>
internal record ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public ApiError? Error { get; init; }
    public string? Message { get; init; }
}

/// <summary>
/// Paged API envelope response.
/// </summary>
internal record ApiPagedResponse<T>
{
    public bool Success { get; init; }
    public List<T> Items { get; init; } = [];
    public PaginationMetadata Pagination { get; init; } = new();
    public ApiError? Error { get; init; }
    public string? Message { get; init; }
}

/// <summary>
/// Error details from the API.
/// </summary>
internal record ApiError
{
    public string Message { get; init; } = default!;
    public string Code { get; init; } = default!;
    public string? Details { get; init; }
}
