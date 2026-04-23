namespace MindOffYou;

/// <summary>
/// What came back.
/// </summary>
public abstract record Reply<TResponseFormat>;

/// <summary>
/// They answered.
/// </summary>
/// <param name="Message">The answered message.</param>
public sealed record Answered<TResponseFormat>(TResponseFormat Message) : Reply<TResponseFormat>;

/// <summary>
/// A communication problem — they couldn't answer.
/// </summary>
/// <param name="Wait">How long to hold before trying again.</param>
public sealed record Unanswered<TResponseFormat>(TimeSpan Wait) : Reply<TResponseFormat>;

/// <summary>
/// We didn't ask.
/// </summary>
/// <param name="Wait">How long to hold before trying again.</param>
public sealed record Unasked<TResponseFormat>(TimeSpan Wait) : Reply<TResponseFormat>;
