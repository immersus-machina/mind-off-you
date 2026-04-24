namespace MindOffYou;

/// <summary>
/// I remember how long to wait between reach-outs while they're struggling.
/// </summary>
public interface IRememberWait
{
    /// <summary>
    /// Recall the current wait, or <paramref name="fallback"/> if none is stored.
    /// </summary>
    Task<TimeSpan> Recall(CareId careId, TimeSpan fallback, CancellationToken ct);

    /// <summary>
    /// Register a new wait.
    /// </summary>
    Task Register(CareId careId, TimeSpan wait, CancellationToken ct);
}
