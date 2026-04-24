namespace MindOffYou;

/// <summary>
/// I remember how each cared-for thing is.
/// </summary>
public interface IRememberCareState
{
    /// <summary>
    /// Recall the current state for <paramref name="careId"/>.
    /// </summary>
    Task<CareState> Recall(CareId careId, CancellationToken ct);

    /// <summary>
    /// Atomically recall, transform, and register.
    /// </summary>
    Task<CareState> Register(CareId careId, Func<CareState, CareState> transform, CancellationToken ct);
}
