using System.Collections.Frozen;

namespace MindOffYou;

internal sealed class CareBuilder : IConfigureCare
{
    private readonly Dictionary<string, Tending> _tendingRegistrations = [];

    public IConfigureCare Mind<TNeedCare>(Tending tending) where TNeedCare : INeedCare
    {
        if (!_tendingRegistrations.TryAdd(TNeedCare.CareId, tending))
        {
            throw new InvalidOperationException(
                $"Tending for '{TNeedCare.CareId}' is already registered.");
        }

        return this;
    }

    public CareRegistrations Build()
    {
        return new(_tendingRegistrations.ToFrozenDictionary());
    }
}
