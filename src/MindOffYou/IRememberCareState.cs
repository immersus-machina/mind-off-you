namespace MindOffYou;

/// <summary>
/// I remember how each cared-for thing is.
/// </summary>
public interface IRememberCareState
{
    /// <summary>
    /// Read the current state for <paramref name="careId"/>.
    /// </summary>
    Task<CareState> Read(CareId careId, CancellationToken ct);

    /// <summary>
    /// Atomically read, transform, and write back.
    /// </summary>
    Task<CareState> Update(CareId careId, Func<CareState, CareState> transform, CancellationToken ct);
}
