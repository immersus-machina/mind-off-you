namespace MindOffYou;

/// <summary>
/// I mind you.
/// </summary>
public interface IMindYou
{
    /// <summary>
    /// How I reach one of those I mind, asking carefully
    /// </summary>
    Task<Reply<TResponseFormat>> Reach<TIn, TResponseFormat>(
        INeedCare<TIn, TResponseFormat> dep,
        TIn request,
        CancellationToken ct)
        where TIn : IRequestCarefully<TResponseFormat>;
}
