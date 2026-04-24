using Microsoft.Extensions.DependencyInjection;

namespace MindOffYou;

/// <summary>
/// Registration for MindOffYou.
/// </summary>
public static class MindOffYouServiceCollectionExtensions
{
    /// <summary>
    /// Add MindOffYou, configuring care through <paramref name="configure"/>.
    /// </summary>
    public static IServiceCollection AddMindOffYou(
        this IServiceCollection services,
        Action<IConfigureCare> configure)
    {
        var builder = new CareBuilder();
        configure(builder);
        services.AddSingleton(builder.Build());
        services.AddSingleton<IMindYou, Minder>();
        return services;
    }
}
