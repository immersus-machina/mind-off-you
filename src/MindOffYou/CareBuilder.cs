using System.Collections.Frozen;

namespace MindOffYou;

internal sealed class CareBuilder : IConfigureCare
{
    private readonly Dictionary<Type, (CareId CareId, Tending Tending)> _registrations = [];

    public IConfigureCare Mind<TNeedCare>(Tending tending) where TNeedCare : INeedCare
    {
        var careId = new CareId(TNeedCare.CareId);

        var conflict = _registrations.FirstOrDefault(kvp => kvp.Value.CareId == careId);
        if (conflict.Key is not null)
        {
            throw new InvalidOperationException(
                $"CareId '{careId}' is already registered by '{conflict.Key.FullName}'; " +
                $"cannot also register '{typeof(TNeedCare).FullName}'.");
        }

        _registrations[typeof(TNeedCare)] = (careId, tending);

        return this;
    }

    public CareRegistrations Build()
    {
        return new(_registrations.ToFrozenDictionary());
    }
}
