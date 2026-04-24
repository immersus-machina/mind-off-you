namespace MindOffYou;

/// <summary>
/// The id of something worth minding.
/// </summary>
public readonly record struct CareId(string Value)
{
    /// <inheritdoc/>
    public override string ToString()
    {
        return Value;
    }
}
