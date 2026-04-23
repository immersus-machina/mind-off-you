namespace MindOffYou;

/// <summary>
/// I mind them on your behalf — ask me to reach, and I'll know whether now's a good moment.
/// </summary>
public interface IMindYou
{
    /// <summary>
    /// How I reach one of those I mind, asking carefully.
    /// </summary>
    Task<Reply<TResponseFormat>> Reach<TIn, TResponseFormat>(
        INeedCare<TIn, TResponseFormat> dep,
        TIn request,
        CancellationToken ct)
        where TIn : IRequestCarefully<TResponseFormat>;
}
