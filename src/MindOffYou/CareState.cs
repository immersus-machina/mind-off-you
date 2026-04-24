namespace MindOffYou;

/// <summary>
/// How they are right now.
/// </summary>
public abstract record CareState;

/// <summary>
/// They're doing fine. I'm out of the way.
/// </summary>
public sealed record Well : CareState;

/// <summary>
/// They've shown some unsteadiness. I'm watching.
/// </summary>
public sealed record Wavering : CareState;

/// <summary>
/// They're struggling — we're giving them space.
/// </summary>
/// <param name="ProbeEligibleAt">When it becomes timely to check in again.</param>
public sealed record Struggling(DateTimeOffset ProbeEligibleAt) : CareState
{
    /// <summary>
    /// Whether it's time to check in again, given <paramref name="now"/>.
    /// </summary>
    public bool IsReadyForCheckIn(DateTimeOffset now) => now >= ProbeEligibleAt;
}

/// <summary>
/// Someone is checking in on them right now.
/// </summary>
public sealed record CheckingIn : CareState;

/// <summary>
/// They're on the mend — recovering. I reach out easefully.
/// </summary>
public sealed record OnTheMend : CareState;
