using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Core7Library.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddCoreSettings<TSettings>(this IHostBuilder builder)
        where TSettings : SettingsBase
    {
        return builder.ConfigureServices((hostBuilderContext, services) =>
        {
            services.AddRequiredSettings<TSettings>(hostBuilderContext.HostingEnvironment);
        });
    }

    public static IHostBuilder AddSettings<TSettings>(this IHostBuilder builder)
        where TSettings : class
    {
        return builder.ConfigureServices((_, services) =>
        {
            services.AddRequiredSettings<TSettings>();
        });
    }
}

public static class HostApplicationBuilderExtensions
{
    public static HostApplicationBuilder AddCoreSettings<TSettings>(this HostApplicationBuilder builder)
        where TSettings : SettingsBase
    {
        builder.Services.AddRequiredSettings<TSettings>(builder.Environment);
        return builder;
    }

    public static HostApplicationBuilder AddSettings<TSettings>(this HostApplicationBuilder builder)
        where TSettings : class
    {
        builder.Services.AddRequiredSettings<TSettings>();
        return builder;
    }
}

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
            .BindConfiguration(nameof(TSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
