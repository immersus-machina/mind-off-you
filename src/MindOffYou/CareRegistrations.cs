using System.Collections.Frozen;

namespace MindOffYou;

internal sealed class CareRegistrations(FrozenDictionary<string, Tending> tendingRegistrations)
{
    public Tending For(string careId)
    {
        return tendingRegistrations.GetValueOrDefault(careId)
            ?? throw new InvalidOperationException(
                $"No tending registered for '{careId}'. " +
                $"Did you {nameof(IConfigureCare.Mind)}<TNeedCare>() in " +
                $"{nameof(MindOffYouServiceCollectionExtensions.AddMindOffYou)}?");
    }
}
