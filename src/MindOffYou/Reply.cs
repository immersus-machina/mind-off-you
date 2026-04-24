namespace MindOffYou;

/// <summary>
/// What came back.
/// </summary>
public abstract record Reply<TMessage>;

/// <summary>
/// They answered.
/// </summary>
/// <param name="Message">The answered message.</param>
public sealed record Answered<TMessage>(TMessage Message) : Reply<TMessage>;

/// <summary>
/// A communication problem — they couldn't answer.
/// </summary>
/// <param name="Wait">How long to hold before trying again.</param>
public sealed record Unanswered<TMessage>(TimeSpan Wait) : Reply<TMessage>;

/// <summary>
/// We didn't ask.
/// </summary>
/// <param name="Wait">How long to hold before trying again.</param>
public sealed record Unasked<TMessage>(TimeSpan Wait) : Reply<TMessage>;
