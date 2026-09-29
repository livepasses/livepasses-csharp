namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for checking in an event pass.
/// </summary>
public class CheckInParams
{
    public RedemptionLocation? Location { get; set; }

    /// <summary>
    /// Free-form key/value pairs recorded with the check-in. Use it for anything you want kept
    /// alongside the check-in, such as an entrance or a staff note.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}
