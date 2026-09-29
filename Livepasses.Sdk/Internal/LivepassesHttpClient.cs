using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Livepasses.Sdk.Exceptions;
using Livepasses.Sdk.Models;

namespace Livepasses.Sdk.Internal;

/// <summary>
/// Internal HTTP client with retry logic, envelope unwrapping, and typed error handling.
/// </summary>
internal class LivepassesHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly int _maxRetries;
    private static readonly Random Rng = new();

    // Only these methods are safe to replay against a 5xx: no SDK request carries an
    // Idempotency-Key, so the retry gate is the HTTP method alone (controller ruling R2).
    private static readonly HashSet<HttpMethod> IdempotentMethods =
        [HttpMethod.Get, HttpMethod.Head, HttpMethod.Put, HttpMethod.Delete];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public LivepassesHttpClient(HttpClient httpClient, string apiKey, int maxRetries)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _maxRetries = maxRetries;
    }

    public async Task<T> GetAsync<T>(string path, Dictionary<string, string?>? queryParams = null)
    {
        var url = BuildUrl(path, queryParams);
        return await RequestAsync<T>(HttpMethod.Get, url).ConfigureAwait(false);
    }

    public async Task<T> PostAsync<T>(string path, object? body = null)
    {
        return await RequestAsync<T>(HttpMethod.Post, path, body).ConfigureAwait(false);
    }

    public async Task PostAsync(string path, object? body = null)
    {
        await RequestWithoutResponseAsync(HttpMethod.Post, path, body).ConfigureAwait(false);
    }

    public async Task<T> PutAsync<T>(string path, object? body = null)
    {
        return await RequestAsync<T>(HttpMethod.Put, path, body).ConfigureAwait(false);
    }

    public async Task PutAsync(string path, object? body = null)
    {
        await RequestWithoutResponseAsync(HttpMethod.Put, path, body).ConfigureAwait(false);
    }

    public async Task DeleteAsync(string path)
    {
        await RequestWithoutResponseAsync(HttpMethod.Delete, path).ConfigureAwait(false);
    }

    public async Task<PagedResponse<T>> GetPagedAsync<T>(string path, Dictionary<string, string?>? queryParams = null)
    {
        var url = BuildUrl(path, queryParams);
        var response = await FetchWithRetryAsync(HttpMethod.Get, url).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var envelope = ReadEnvelope<ApiPagedResponse<T>>(json);

        if (!response.IsSuccessStatusCode || envelope is { Success: false })
            throw ToError(response, envelope?.Error);

        if (envelope is null)
            throw new LivepassesException("Failed to deserialize API response", (int)response.StatusCode, "DESERIALIZATION_ERROR");

        return new PagedResponse<T>
        {
            Items = envelope.Items,
            Pagination = envelope.Pagination
        };
    }

    private async Task<T> RequestAsync<T>(HttpMethod method, string path, object? body = null)
    {
        var response = await FetchWithRetryAsync(method, path, body).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var envelope = ReadEnvelope<ApiResponse<T>>(json);

        if (!response.IsSuccessStatusCode || envelope is { Success: false })
            throw ToError(response, envelope?.Error);

        if (envelope is null)
            throw new LivepassesException("Failed to deserialize API response", (int)response.StatusCode, "DESERIALIZATION_ERROR");

        return envelope.Data!;
    }

    private async Task RequestWithoutResponseAsync(HttpMethod method, string path, object? body = null)
    {
        var response = await FetchWithRetryAsync(method, path, body).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var envelope = ReadEnvelope<ApiResponse<object>>(json);

        if (!response.IsSuccessStatusCode || envelope is { Success: false })
            throw ToError(response, envelope?.Error);
    }

    private static TEnvelope? ReadEnvelope<TEnvelope>(string json) where TEnvelope : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try { return JsonSerializer.Deserialize<TEnvelope>(json, JsonOptions); }
        catch (JsonException) { return null; } // empty (challenge 401) or non-JSON body (proxy error page)
    }

    private static LivepassesException ToError(HttpResponseMessage response, ApiError? error) =>
        ExceptionFactory.Create(
            error?.Message ?? $"API request failed with status {(int)response.StatusCode}",
            (int)response.StatusCode,
            error?.Code ?? "GENERAL_ERROR",
            error?.Details,
            ParseRetryAfter(response.Headers),
            error?.Fields);

    private async Task<HttpResponseMessage> FetchWithRetryAsync(HttpMethod method, string path, object? body = null)
    {
        var maxAttempts = _maxRetries + 1;
        HttpResponseMessage? lastResponse = null;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var request = new HttpRequestMessage(method, path);
            request.Headers.Add("X-API-Key", _apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (body is not null)
            {
                var jsonBody = JsonSerializer.Serialize(body, JsonOptions);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                throw new LivepassesException("Request timed out", 0, "TIMEOUT");
            }
            catch (HttpRequestException) when (attempt < maxAttempts)
            {
                await DelayAsync(GetBackoffDelay(attempt)).ConfigureAwait(false);
                continue;
            }
            catch (HttpRequestException)
            {
                throw new LivepassesException("Network error", 0, "NETWORK_ERROR");
            }

            lastResponse = response;

            // Handle 429 Rate Limit
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                if (attempt >= maxAttempts) break;

                var retryAfterSeconds = ParseRetryAfter(response.Headers);
                var delay = retryAfterSeconds.HasValue
                    ? retryAfterSeconds.Value * 1000
                    : GetBackoffDelay(attempt);

                await DelayAsync(delay).ConfigureAwait(false);
                continue;
            }

            // Handle 5xx Server Error — only for idempotent methods, cap at 2 retries (3 total attempts)
            if ((int)response.StatusCode >= 500 && IdempotentMethods.Contains(method))
            {
                var maxServerRetries = Math.Min(maxAttempts, 3);
                if (attempt < maxServerRetries)
                {
                    await DelayAsync(GetBackoffDelay(attempt)).ConfigureAwait(false);
                    continue;
                }
            }

            // Return for all other status codes (success or client errors)
            return response;
        }

        // Exhausted all attempts — return last response for error handling
        return lastResponse ?? throw new LivepassesException("Network error after retries", 0, "NETWORK_ERROR");
    }

    private static string BuildUrl(string path, Dictionary<string, string?>? queryParams)
    {
        if (queryParams is null || queryParams.Count == 0)
            return path;

        var sb = new StringBuilder(path);
        var first = true;

        foreach (var (key, value) in queryParams)
        {
            if (value is null) continue;

            sb.Append(first ? '?' : '&');
            sb.Append(Uri.EscapeDataString(key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(value));
            first = false;
        }

        return sb.ToString();
    }

    private static int GetBackoffDelay(int attempt)
    {
        var baseDelay = Math.Min(1000 * (int)Math.Pow(2, attempt - 1), 30000);
        var jitter = Rng.Next(0, 500);
        return baseDelay + jitter;
    }

    private static int? ParseRetryAfter(HttpResponseHeaders headers)
    {
        if (headers.TryGetValues("Retry-After", out var values))
        {
            var value = values.FirstOrDefault();
            if (value is not null && int.TryParse(value, out var seconds))
                return seconds;
        }

        return null;
    }

    private static Task DelayAsync(int milliseconds)
    {
        return Task.Delay(milliseconds);
    }
}
