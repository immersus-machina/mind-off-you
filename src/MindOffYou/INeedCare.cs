namespace MindOffYou;

/// <summary>
/// I'm someone worth reaching out to carefully.
/// </summary>
public interface INeedCare
{
    /// <summary>
    /// My id. Sharing it is sharing the same care.
    /// </summary>
    static abstract string CareId { get; }
}

/// <summary>
/// I'm someone worth reaching out to carefully.
/// </summary>
/// <remarks>
/// The same <see cref="INeedCare.CareId"/> shares the same care.
/// </remarks>
public interface INeedCare<TRequest, TMessage> : INeedCare
    where TRequest : IRequestCarefully<TMessage>
{
    /// <summary>
    /// How I answer. Called only when someone has decided it's a good time to reach out.
    /// </summary>
    Task<TMessage> HandleCarefully(TRequest input, CancellationToken ct);

    /// <summary>
    /// What I make of it when I'm unheard. Override to log through your own channels; the default writes to the console so failures aren't silent.
    /// </summary>
    void WhenUnheard(TRequest input, Exception cause)
    {
        Console.Error.WriteLine($"[MindOffYou] {GetType().Name} unheard: {cause}");
    }
}
