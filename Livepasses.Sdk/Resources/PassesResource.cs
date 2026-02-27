using Livepasses.Sdk.Internal;
using Livepasses.Sdk.Models;

namespace Livepasses.Sdk.Resources;

/// <summary>
/// Resource for managing passes — generation, lookup, validation, redemption, and batch operations.
/// </summary>
public class PassesResource
{
    private readonly LivepassesHttpClient _http;

    internal PassesResource(LivepassesHttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Generate passes for a template.
    /// </summary>
    public async Task<PassGenerationResult> GenerateAsync(GeneratePassesParams parameters)
    {
        return await _http.PostAsync<PassGenerationResult>("/api/passes/generate", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Generate passes and wait for completion, polling if async processing is used.
    /// </summary>
    public async Task<PassGenerationResult> GenerateAndWaitAsync(
        GeneratePassesParams parameters,
        GenerateAndWaitOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var result = await GenerateAsync(parameters).ConfigureAwait(false);

        if (!result.IsAsyncProcessing)
            return result;

        var batchId = result.BatchId;
        var pollInterval = options?.PollInterval ?? 2000;
        var maxAttempts = options?.MaxAttempts ?? 150;

        var batchStatus = await Polling.PollUntilCompleteAsync(
            () => GetBatchStatusAsync(batchId),
            status => status.IsCompleted,
            TimeSpan.FromMilliseconds(pollInterval),
            maxAttempts,
            options?.OnProgress,
            cancellationToken).ConfigureAwait(false);

        // Map batch generated passes to GeneratedPass objects
        var passes = (batchStatus.GeneratedPasses ?? []).Select(gp => new GeneratedPass
        {
            Id = gp.Id,
            CustomerEmail = gp.HolderEmail,
            ConfirmationCode = null,
            Platforms = new PassPlatforms
            {
                Apple = new PassPlatform { Available = gp.HasApplePass, Features = [] },
                Google = new PassPlatform { Available = gp.HasGooglePass, Features = [] }
            },
            BusinessData = new UnifiedBusinessData(),
            QrCode = null,
            Status = gp.Status,
            Analytics = null
        }).ToList();

        return result with { Passes = passes };
    }

    /// <summary>
    /// List passes with pagination and filtering.
    /// </summary>
    public async Task<PagedResponse<GlobalPassDto>> ListAsync(ListPassesParams? parameters = null)
    {
        var queryParams = new Dictionary<string, string?>();

        if (parameters is not null)
        {
            queryParams["templateId"] = parameters.TemplateId;
            queryParams["status"] = parameters.Status;
            queryParams["platform"] = parameters.Platform;
            queryParams["page"] = parameters.Page?.ToString();
            queryParams["pageSize"] = parameters.PageSize?.ToString();
            queryParams["searchTerm"] = parameters.SearchTerm;
            queryParams["sortBy"] = parameters.SortBy;
            queryParams["sortDescending"] = parameters.SortDescending?.ToString().ToLowerInvariant();
        }

        return await _http.GetPagedAsync<GlobalPassDto>("/api/passes", queryParams).ConfigureAwait(false);
    }

    /// <summary>
    /// Lazily iterate through all passes across all pages.
    /// </summary>
    public IAsyncEnumerable<GlobalPassDto> ListAutoPaginateAsync(
        ListPassesParams? parameters = null,
        CancellationToken cancellationToken = default)
    {
        return PaginationHelper.AutoPaginateAsync(
            page => ListAsync(new ListPassesParams
            {
                TemplateId = parameters?.TemplateId,
                Status = parameters?.Status,
                Platform = parameters?.Platform,
                Page = page,
                PageSize = parameters?.PageSize,
                SearchTerm = parameters?.SearchTerm,
                SortBy = parameters?.SortBy,
                SortDescending = parameters?.SortDescending
            }),
            cancellationToken);
    }

    /// <summary>
    /// Look up a pass by ID or pass number.
    /// </summary>
    public async Task<PassLookupResult> LookupAsync(LookupPassParams parameters)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["passId"] = parameters.PassId,
            ["passNumber"] = parameters.PassNumber
        };

        return await _http.GetAsync<PassLookupResult>("/api/passes/lookup", queryParams).ConfigureAwait(false);
    }

    /// <summary>
    /// Validate a pass for redemption.
    /// </summary>
    public async Task<PassValidationResult> ValidateAsync(string passId)
    {
        return await _http.GetAsync<PassValidationResult>($"/api/passes/{passId}/validate").ConfigureAwait(false);
    }

    /// <summary>
    /// Update a pass.
    /// </summary>
    public async Task UpdateAsync(string passId, UpdatePassParams parameters)
    {
        await _http.PutAsync($"/api/passes/{passId}", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Bulk-update multiple passes.
    /// </summary>
    public async Task BulkUpdateAsync(BulkUpdatePassesParams parameters)
    {
        await _http.PostAsync("/api/passes/bulk-update", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Redeem a pass.
    /// </summary>
    public async Task<PassRedemptionResult> RedeemAsync(string passId, RedeemPassParams? parameters = null)
    {
        return await _http.PostAsync<PassRedemptionResult>($"/api/passes/{passId}/redeem", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Check in an event pass.
    /// </summary>
    public async Task<PassRedemptionResult> CheckInAsync(string passId, CheckInParams? parameters = null)
    {
        return await _http.PostAsync<PassRedemptionResult>($"/api/passes/{passId}/check-in", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Redeem a coupon pass.
    /// </summary>
    public async Task<PassRedemptionResult> RedeemCouponAsync(string passId, RedeemCouponParams? parameters = null)
    {
        return await _http.PostAsync<PassRedemptionResult>($"/api/passes/{passId}/redeem-coupon", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Perform a loyalty points transaction on a pass.
    /// </summary>
    public async Task<PassRedemptionResult> LoyaltyTransactAsync(string passId, LoyaltyTransactionParams parameters)
    {
        return await _http.PostAsync<PassRedemptionResult>($"/api/passes/{passId}/loyalty/transact", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Get the status of a batch operation.
    /// </summary>
    public async Task<BatchStatusResult> GetBatchStatusAsync(string batchId)
    {
        return await _http.GetAsync<BatchStatusResult>($"/api/passes/batch/{batchId}/status").ConfigureAwait(false);
    }
}
