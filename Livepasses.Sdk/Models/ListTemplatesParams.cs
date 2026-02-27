namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for listing templates.
/// </summary>
public class ListTemplatesParams
{
    public string? Type { get; set; }
    public string? Status { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool? SortDescending { get; set; }
}
