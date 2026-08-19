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
    /// Push a scoped update to all eligible passes of a template.
    /// </summary>
    public async Task PushTemplateAsync(string templateId, PushTemplatePassesParams parameters)
    {
        await _http.PostAsync($"/api/passes/template/{templateId}/push", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Redeem a single-use pass.
    /// </summary>
    /// <remarks>
    /// Redeeming is terminal, so a pass the holder is meant to keep using must not go through
    /// here. Multi-use passes are refused with <c>422</c> / <c>OPERATION_NOT_ALLOWED</c> instead
    /// of being consumed. Use the operation built for the type:
    /// <list type="bullet">
    ///   <item>loyalty and stamp cards: <c>StampAsync</c> (and <c>UnstampAsync</c> to undo)</item>
    ///   <item>memberships: <c>MembershipCheckInAsync</c>, which does not consume the pass</item>
    ///   <item>coupons that allow multiple redemptions: <c>RedeemCouponAsync</c></item>
    ///   <item>gift cards: <c>RedeemGiftCardAsync</c>, which takes the amount to deduct</item>
    /// </list>
    /// </remarks>
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
    /// Deduct an amount from a gift card's balance. A redemption for more than the remaining
    /// balance is rejected, and the balance is left untouched in that case.
    /// </summary>
    public async Task<PassRedemptionResult> RedeemGiftCardAsync(string passId, RedeemGiftCardParams parameters)
    {
        return await _http.PostAsync<PassRedemptionResult>($"/api/passes/{passId}/giftcard/redeem", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Check in a membership pass. Unlike an event check-in the pass is NOT consumed — it stays
    /// valid for the next visit. On a quota-limited membership the remaining uses decrement, and a
    /// check-in at zero is denied.
    /// </summary>
    public async Task<PassRedemptionResult> MembershipCheckInAsync(string passId, MembershipCheckInParams? parameters = null)
    {
        return await _http.PostAsync<PassRedemptionResult>($"/api/passes/{passId}/membership/check-in", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Add one stamp to the stamp card behind this pass. Repeat stamps on the same card are refused
    /// inside a short cooldown, so a double scan at the till does not award two stamps.
    /// </summary>
    public async Task<PassRedemptionResult> StampAsync(string passId)
    {
        // Passes an empty object, not null: the endpoint binds a request DTO, and a null body sends
        // no content and no Content-Type, which FastEndpoints answers with 415.
        return await _http.PostAsync<PassRedemptionResult>($"/api/passes/{passId}/stamp", new { }).ConfigureAwait(false);
    }

    /// <summary>
    /// Take back the most recent stamp, for correcting a mis-scan. Refused when there is nothing to
    /// undo, or when the last stamp was paid for by an external order.
    /// </summary>
    public async Task<PassRedemptionResult> UnstampAsync(string passId)
    {
        // See StampAsync: an empty object rather than null, or the request-DTO endpoint answers 415.
        return await _http.PostAsync<PassRedemptionResult>($"/api/passes/{passId}/unstamp", new { }).ConfigureAwait(false);
    }

    /// <summary>
    /// Resolve a scanned barcode or NFC tap value and redeem it in one call.
    /// </summary>
    public async Task<PassRedemptionResult> RedeemByScanAsync(RedeemByScanParams parameters)
    {
        return await _http.PostAsync<PassRedemptionResult>("/api/passes/redeem-by-scan", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Get the status of a batch operation.
    /// </summary>
    public async Task<BatchStatusResult> GetBatchStatusAsync(string batchId)
    {
        return await _http.GetAsync<BatchStatusResult>($"/api/passes/batch/{batchId}/status").ConfigureAwait(false);
    }
}
