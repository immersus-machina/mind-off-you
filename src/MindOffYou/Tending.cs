namespace MindOffYou;

/// <summary>
/// How I tend to you.
/// </summary>
/// <remarks>
/// I judge impact by what I <see cref="ListenFor"/> within <see cref="NoticeOver"/>. When you
/// <see cref="SeemStrugglingAt"/> the brink I <see cref="GiveSpace"/>. Between retries I
/// <see cref="HoldAtLeast"/>, at worst <see cref="HoldAtMost"/> — then I
/// <see cref="ReturnWith"/> care, and consider you <see cref="FullyBackAt"/> ease.
/// </remarks>
public sealed record Tending
{
    /// <summary>
    /// The share of recent failures at which I read you as struggling.
    /// </summary>
    public double SeemStrugglingAt { get; init; } = 0.5;

    /// <summary>
    /// How many attempts I listen for before I judge.
    /// </summary>
    public int ListenFor { get; init; } = 10;

    /// <summary>
    /// How far back I look when judging.
    /// </summary>
    public TimeSpan NoticeOver { get; init; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// How long I give you space once I've decided.
    /// </summary>
    public TimeSpan GiveSpace { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// The shortest I'll hold before asking again while you're out.
    /// </summary>
    public TimeSpan HoldAtLeast { get; init; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// The longest I'll hold before asking again while you're out.
    /// </summary>
    public TimeSpan HoldAtMost { get; init; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// How gently I return — how many at once when I first come back.
    /// </summary>
    public int ReturnWith { get; init; } = 2;

    /// <summary>
    /// How many at once before I trust you're well again.
    /// </summary>
    public int FullyBackAt { get; init; } = 16;
}
