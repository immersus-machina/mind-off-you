using System.Collections.Frozen;

namespace MindOffYou;

internal sealed class CareRegistrations(FrozenDictionary<CareId, Tending> tendingRegistrations)
{
    public Tending For(CareId careId)
    {
        return tendingRegistrations.GetValueOrDefault(careId)
            ?? throw new InvalidOperationException(
                $"No tending registered for '{careId}'. " +
                $"Did you {nameof(IConfigureCare.Mind)}<TNeedCare>() in " +
                $"{nameof(MindOffYouServiceCollectionExtensions.AddMindOffYou)}?");
    }
}
