using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Core7Library.Extensions;

public static class ServiceCollectionExtensions
{
    public static OptionsBuilder<TSettings> AddRequiredSettings<TSettings>(this IServiceCollection services, IHostEnvironment hostEnvironment)
        where TSettings : SettingsBase
    {
        return services
            .AddRequiredSettings<TSettings>()
            .Configure(s => s.EnvironmentName = EnvironmentName.FromValue(hostEnvironment.EnvironmentName));
    }

    public static OptionsBuilder<TSettings> AddRequiredSettings<TSettings>(this IServiceCollection services)
        where TSettings : class
    {
        return services.AddOptions<TSettings>()
            .BindConfiguration(typeof(TSettings).Name)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
