using System.Runtime.CompilerServices;
using Livepasses.Sdk.Models;

namespace Livepasses.Sdk.Internal;

/// <summary>
/// Helper for automatic pagination using IAsyncEnumerable.
/// </summary>
internal static class PaginationHelper
{
    /// <summary>
    /// Lazily iterates through all pages, yielding items one at a time.
    /// </summary>
    public static async IAsyncEnumerable<T> AutoPaginateAsync<T>(
        Func<int, Task<PagedResponse<T>>> fetchPage,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var response = await fetchPage(page).ConfigureAwait(false);

            foreach (var item in response.Items)
                yield return item;

            if (page >= response.Pagination.TotalPages || response.Items.Count == 0)
                break;

            page++;
        }
    }
}
