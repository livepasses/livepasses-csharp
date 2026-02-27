namespace Livepasses.Sdk.Models;

/// <summary>
/// Platform availability for a pass.
/// </summary>
public record PassPlatforms
{
    public PassPlatform Apple { get; init; } = new();
    public PassPlatform Google { get; init; } = new();
}
