namespace MindOffYou;

/// <summary>
/// What came back.
/// </summary>
public abstract record Reply<TOut>;

/// <summary>
/// They answered.
/// </summary>
/// <param name="Message">The answered message.</param>
public sealed record Answered<TOut>(TOut Message) : Reply<TOut>;

/// <summary>
/// A communication problem — they couldn't answer.
/// </summary>
/// <param name="Wait">How long to hold before trying again.</param>
public sealed record Unanswered<TOut>(TimeSpan Wait) : Reply<TOut>;

/// <summary>
/// We didn't ask.
/// </summary>
/// <param name="Wait">How long to hold before trying again.</param>
public sealed record Unasked<TOut>(TimeSpan Wait) : Reply<TOut>;
