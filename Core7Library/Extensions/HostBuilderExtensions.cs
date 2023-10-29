using Microsoft.Extensions.Hosting;

namespace Core7Library.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddBaseSettings<TSettings>(this IHostBuilder builder)
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
    public static HostApplicationBuilder AddBaseSettings<TSettings>(this HostApplicationBuilder builder)
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
