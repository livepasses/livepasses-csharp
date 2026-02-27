namespace Livepasses.Sdk.Models;

/// <summary>
/// Platform-specific pass information (Apple or Google).
/// </summary>
public record PassPlatform
{
    public bool Available { get; init; }
    public string? AddToWalletUrl { get; init; }
    public string? PassUrl { get; init; }
    public string? JwtToken { get; init; }
    public List<string> Features { get; init; } = [];
}
