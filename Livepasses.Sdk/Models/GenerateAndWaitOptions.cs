namespace Livepasses.Sdk.Models;

/// <summary>
/// Options for the generate-and-wait polling behavior.
/// </summary>
public class GenerateAndWaitOptions
{
    /// <summary>Polling interval in milliseconds. Default: 2000.</summary>
    public int PollInterval { get; set; } = 2000;

    /// <summary>Maximum number of poll attempts. Default: 150.</summary>
    public int MaxAttempts { get; set; } = 150;

    /// <summary>Called after each poll with the current batch status.</summary>
    public Action<BatchStatusResult>? OnProgress { get; set; }
}
