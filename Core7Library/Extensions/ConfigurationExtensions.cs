using Microsoft.Extensions.Configuration;

namespace Core7Library.Extensions;

public static class ConfigurationExtensions
{
    public static TConfigSettings GetRequiredConfig<TConfigSettings>(this IConfiguration config)
    {
        string configSettingsName = typeof(TConfigSettings).Name;
        TConfigSettings? configSettings = config.GetSection(configSettingsName).Get<TConfigSettings>();
        if (configSettings == null) throw new ApplicationException($"{configSettingsName} required, but missing!");
        return configSettings;
    }
}
