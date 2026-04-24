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
}
