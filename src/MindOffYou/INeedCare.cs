namespace MindOffYou;

/// <summary>
/// I'm someone worth reaching out to carefully.
/// </summary>
public interface INeedCare
{
    /// <summary>
    /// My id. Sharing it is sharing the same consideration.
    /// </summary>
    static abstract string CareId { get; }
}

/// <summary>
/// I'm someone worth reaching out to carefully.
/// </summary>
/// <remarks>
/// The same <see cref="INeedCare.CareId"/> shares the same consideration.
/// </remarks>
public interface INeedCare<TIn, TOut> : INeedCare
    where TIn : IRequestCarefully<TOut>
{
    /// <summary>
    /// What to handle carefully.
    /// </summary>
    Task<TOut> HandleCarefully(TIn input, CancellationToken ct);
}
