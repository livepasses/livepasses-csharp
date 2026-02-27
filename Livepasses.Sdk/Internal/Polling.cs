namespace Livepasses.Sdk.Internal;

/// <summary>
/// Polling utility for waiting on async operations to complete.
/// </summary>
internal static class Polling
{
    /// <summary>
    /// Polls a function until the result satisfies the completion predicate.
    /// </summary>
    public static async Task<T> PollUntilCompleteAsync<T>(
        Func<Task<T>> fn,
        Func<T, bool> isComplete,
        TimeSpan? interval = null,
        int maxAttempts = 150,
        Action<T>? onProgress = null,
        CancellationToken cancellationToken = default)
    {
        var delay = interval ?? TimeSpan.FromMilliseconds(2000);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await fn().ConfigureAwait(false);
            onProgress?.Invoke(result);

            if (isComplete(result))
                return result;

            if (attempt < maxAttempts)
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }

        // Final attempt after loop exhaustion
        var finalResult = await fn().ConfigureAwait(false);
        onProgress?.Invoke(finalResult);
        return finalResult;
    }
}
