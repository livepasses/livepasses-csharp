namespace Livepasses.Sdk.Models;

/// <summary>
/// Pagination metadata from the API.
/// </summary>
public record PaginationMetadata
{
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public int TotalItems { get; init; }
}
