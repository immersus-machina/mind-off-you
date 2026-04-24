namespace MindOffYou;

/// <summary>
/// I mind them on your behalf — ask me to reach, and I'll know whether now's a good moment.
/// </summary>
public interface IMindYou
{
    /// <summary>
    /// How I reach one of those I mind, asking carefully.
    /// </summary>
    /// <exception cref="OperationCanceledException">If <paramref name="ct"/> is cancelled.</exception>
    Task<Reply<TResponseFormat>> Reach<TRequestFormat, TResponseFormat>(
        INeedCare<TRequestFormat, TResponseFormat> needCare,
        TRequestFormat request,
        CancellationToken ct)
        where TRequestFormat : IRequestCarefully<TResponseFormat>;
}
