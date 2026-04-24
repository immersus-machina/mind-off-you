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
    /// Note that they were heard at <paramref name="now"/>.
    /// </summary>
    Task NoteHeard(CareId careId, DateTimeOffset now, Tending tending, CancellationToken ct);

    /// <summary>
    /// Note that they weren't heard at <paramref name="now"/>.
    /// </summary>
    Task NoteUnheard(CareId careId, DateTimeOffset now, Tending tending, CancellationToken ct);
}
