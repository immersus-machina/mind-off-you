namespace MindOffYou;

/// <summary>
/// How they are right now.
/// </summary>
public abstract record CareState;

/// <summary>
/// They're doing fine.
/// </summary>
public sealed record Well : CareState;

/// <summary>
/// They're struggling — we're giving them space.
/// </summary>
public sealed record Struggling : CareState;

/// <summary>
/// Someone is checking in on them right now.
/// </summary>
public sealed record CheckingIn : CareState;
