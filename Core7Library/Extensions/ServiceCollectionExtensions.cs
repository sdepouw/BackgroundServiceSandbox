using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Core7Library.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up <see cref="IOptions{TOptions}"/> for <typeparam name="TSettings" />, validates any annotations,
    /// and initializes <see cref="SettingsBase.EnvironmentName" />.
    /// </summary>
    public static OptionsBuilder<TSettings> AddRequiredSettings<TSettings>(this IServiceCollection services, IHostEnvironment hostEnvironment)
        where TSettings : SettingsBase
    {
        return services
            .AddRequiredSettings<TSettings>()
            .Configure(s => s.EnvironmentName = EnvironmentName.FromValue(hostEnvironment.EnvironmentName));
    }

    /// <summary>
    /// Sets up <see cref="IOptions{TOptions}"/> for <typeparam name="TSettings" /> and validates any annotations.
    /// </summary>
    public static OptionsBuilder<TSettings> AddRequiredSettings<TSettings>(this IServiceCollection services)
        where TSettings : class
    {
        return services.AddOptions<TSettings>()
            .BindConfiguration(typeof(TSettings).Name)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
