namespace MindOffYou;

/// <summary>
/// I configure care.
/// </summary>
public interface IConfigureCare
{
    /// <summary>
    /// Mind <typeparamref name="TNeedCare"/> this way.
    /// </summary>
    IConfigureCare Mind<TNeedCare>(Tending tending) where TNeedCare : INeedCare;
}
