using Livepasses.Sdk.Internal;
using Livepasses.Sdk.Models;

namespace Livepasses.Sdk.Resources;

/// <summary>
/// Resource for managing pass templates.
/// </summary>
public class TemplatesResource
{
    private readonly LivepassesHttpClient _http;

    internal TemplatesResource(LivepassesHttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// List templates with pagination and filtering.
    /// </summary>
    public async Task<PagedResponse<TemplateListItem>> ListAsync(ListTemplatesParams? parameters = null)
    {
        var queryParams = new Dictionary<string, string?>();

        if (parameters is not null)
        {
            queryParams["type"] = parameters.Type;
            queryParams["status"] = parameters.Status;
            queryParams["page"] = parameters.Page?.ToString();
            queryParams["pageSize"] = parameters.PageSize?.ToString();
            queryParams["searchTerm"] = parameters.SearchTerm;
            queryParams["sortBy"] = parameters.SortBy;
            queryParams["sortDescending"] = parameters.SortDescending?.ToString().ToLowerInvariant();
        }

        return await _http.GetPagedAsync<TemplateListItem>("/api/templates", queryParams).ConfigureAwait(false);
    }

    /// <summary>
    /// Get a template by ID.
    /// </summary>
    public async Task<TemplateDetail> GetAsync(string templateId)
    {
        return await _http.GetAsync<TemplateDetail>($"/api/templates/{templateId}").ConfigureAwait(false);
    }

    /// <summary>
    /// Create a new template.
    /// </summary>
    public async Task<TemplateDetail> CreateAsync(CreateTemplateParams parameters)
    {
        return await _http.PostAsync<TemplateDetail>("/api/templates", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Update an existing template.
    /// </summary>
    public async Task<TemplateDetail> UpdateAsync(string templateId, UpdateTemplateParams parameters)
    {
        return await _http.PutAsync<TemplateDetail>($"/api/templates/{templateId}", parameters).ConfigureAwait(false);
    }

    /// <summary>
    /// Activate a template.
    /// </summary>
    public async Task ActivateAsync(string templateId)
    {
        await _http.PostAsync($"/api/templates/{templateId}/activate").ConfigureAwait(false);
    }

    /// <summary>
    /// Deactivate a template.
    /// </summary>
    public async Task DeactivateAsync(string templateId)
    {
        await _http.PostAsync($"/api/templates/{templateId}/deactivate").ConfigureAwait(false);
    }
}
