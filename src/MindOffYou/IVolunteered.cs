namespace MindOffYou;

/// <summary>
/// Tells if I <see cref="ShouldProceed"/>. Let me know when done.
/// </summary>
public interface IVolunteered : IAsyncDisposable
{
    /// <summary>
    /// Of several volunteering at the same moment, only one proceeds.
    /// </summary>
    bool ShouldProceed { get; }
}
