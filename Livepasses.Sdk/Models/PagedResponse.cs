namespace Livepasses.Sdk.Models;

/// <summary>
/// SDK-facing paginated response containing items and pagination metadata.
/// </summary>
public record PagedResponse<T>
{
    /// <summary>Items on this page.</summary>
    public List<T> Items { get; init; } = [];

    /// <summary>Pagination metadata.</summary>
    public PaginationMetadata Pagination { get; init; } = new();
}
