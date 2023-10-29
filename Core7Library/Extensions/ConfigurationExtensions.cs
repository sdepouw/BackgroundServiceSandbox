using Microsoft.Extensions.Configuration;

namespace Core7Library.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets strongly-typed <typeparamref name="TSettings" /> from configuration
    /// </summary>
    /// <exception cref="ApplicationException">Thrown when expected configuration section is missing</exception>
    public static TSettings GetRequiredSettings<TSettings>(this IConfiguration config)
        where TSettings : class
    {
        string settingsName = typeof(TSettings).Name;
        TSettings? settings = config.GetSection(settingsName).Get<TSettings>();
        if (settings == null) throw new ApplicationException($"{settingsName} required, but missing!");
        return settings;
    }
}
