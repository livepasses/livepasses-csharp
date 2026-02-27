namespace Livepasses.Sdk.Models;

/// <summary>
/// Event-specific business context.
/// </summary>
public class EventContext
{
    public string? EventName { get; set; }
    public string? EventDate { get; set; }
    public string? DoorsOpen { get; set; }
    public string? SpecialAnnouncement { get; set; }
}
