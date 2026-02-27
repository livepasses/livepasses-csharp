namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for listing passes with pagination and filtering.
/// </summary>
public class ListPassesParams
{
    public string? TemplateId { get; set; }
    public string? Status { get; set; }
    public string? Platform { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool? SortDescending { get; set; }
}
