namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for checking in a membership pass.
///
/// Unlike an event check-in the pass is NOT consumed — it stays valid for the next visit. On a
/// quota-limited membership the remaining uses decrement, and a check-in at zero is denied.
/// </summary>
public class MembershipCheckInParams
{
    public string? Gate { get; set; }
    public string? RedemptionMethod { get; set; }
    public RedemptionLocation? Location { get; set; }
}
