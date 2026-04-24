using System.Collections.Frozen;

namespace MindOffYou;

internal sealed class CareBuilder : IConfigureCare
{
    private readonly Dictionary<CareId, Tending> _tendingRegistrations = [];

    public IConfigureCare Mind<TNeedCare>(Tending tending) where TNeedCare : INeedCare
    {
        var careId = new CareId(TNeedCare.CareId);
        if (!_tendingRegistrations.TryAdd(careId, tending))
        {
            throw new InvalidOperationException(
                $"Tending for '{careId}' is already registered.");
        }

        return this;
    }

    public CareRegistrations Build()
    {
        return new(_tendingRegistrations.ToFrozenDictionary());
    }
}
