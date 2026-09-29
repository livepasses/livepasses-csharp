namespace Livepasses.Sdk.Models;

/// <summary>
/// Parameters for updating a pass (<c>PUT /api/passes/{id}</c>).
///
/// Send a non-empty <see cref="UpdatedFields"/>, a non-empty <see cref="MessageBody"/>, or both.
/// The API refuses any other body field with a 400.
/// </summary>
public class UpdatePassParams
{
    /// <summary>
    /// The field changes, keyed by updatable field name (for example <c>validUntil</c>,
    /// <c>memberTier</c>, <c>points</c>). Which fields are updatable depends on the pass type.
    /// </summary>
    public Dictionary<string, object>? UpdatedFields { get; set; }

    /// <summary>Why the pass changed, recorded in its history (at most 500 characters).</summary>
    public string? Reason { get; set; }

    /// <summary>Header of the notification shown to the holder (at most 80 characters).</summary>
    public string? MessageHeader { get; set; }

    /// <summary>
    /// Body of the notification shown to the holder (at most 2000 characters). Replaces the
    /// automatic summary of the changed fields.
    /// </summary>
    public string? MessageBody { get; set; }

    /// <summary>
    /// <c>false</c> updates the pass silently. Left unset, a field change notifies the holder
    /// with an automatic summary.
    /// </summary>
    public bool? Notify { get; set; }
}
