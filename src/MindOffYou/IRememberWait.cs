namespace MindOffYou;

/// <summary>
/// I remember how long to wait between reach-outs while they're struggling.
/// </summary>
public interface IRememberWait
{
    /// <summary>
    /// Read the current wait, or <paramref name="fallback"/> if none is stored.
    /// </summary>
    Task<TimeSpan> Read(CareId careId, TimeSpan fallback, CancellationToken ct);

    /// <summary>
    /// Write the new wait.
    /// </summary>
    Task Write(CareId careId, TimeSpan wait, CancellationToken ct);
}
