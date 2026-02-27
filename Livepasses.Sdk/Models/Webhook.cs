namespace Livepasses.Sdk.Models;

/// <summary>
/// A webhook subscription.
/// </summary>
public record Webhook
{
    public string Id { get; init; } = default!;
    public string Url { get; init; } = default!;
    public List<string> Events { get; init; } = [];
    public bool IsActive { get; init; }
    public string CreatedAt { get; init; } = default!;
    public string? Secret { get; init; }
}
