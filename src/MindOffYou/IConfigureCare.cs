namespace MindOffYou;

/// <summary>
/// I'm where you say, up front, how each dependency should be cared for.
/// </summary>
public interface IConfigureCare
{
    /// <summary>
    /// Mind <typeparamref name="TNeedCare"/> this way.
    /// </summary>
    IConfigureCare Mind<TNeedCare>(Tending tending) where TNeedCare : INeedCare;
}
