using System.Collections.Frozen;

namespace MindOffYou;

internal sealed class CareRegistrations(FrozenDictionary<Type, (CareId CareId, Tending Tending)> byType)
{
    public (CareId CareId, Tending Tending) For(Type type)
    {
        if (!byType.TryGetValue(type, out var registration))
        {
            throw new InvalidOperationException(
                $"No care registered for type '{type.FullName}'. " +
                $"Did you {nameof(IConfigureCare.Mind)}<{type.Name}>() in " +
                $"{nameof(MindOffYouServiceCollectionExtensions.AddMindOffYou)}?");
        }
        return registration;
    }
}
