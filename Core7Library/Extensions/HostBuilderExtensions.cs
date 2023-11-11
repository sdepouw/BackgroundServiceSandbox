using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Refit;

namespace Core7Library.Extensions;

/// <summary>
/// Extensions for .NET 7's host builder.
/// </summary>
public static class HostBuilderExtensions
{
    public static IHostBuilder AddSettings<TSettings>(this IHostBuilder builder)
        where TSettings : class
    {
        return builder.ConfigureServices((_, services) =>
        {
            services.AddRequiredSettings<TSettings>();
        });
    }
}

/// <summary>
/// Extensions for for .NET 7's host builder context.
/// </summary>
public static class HostBuilderContextExtensions
{
    public static TSettings GetRequiredSettings<TSettings>(this HostBuilderContext hostBuilderContext)
        where TSettings : class
    {
        return hostBuilderContext.Configuration.GetRequiredSettings<TSettings>();
    }
}

/// <summary>
/// Extensions for .NET 8's host builder.
/// </summary>
public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Sets up <see cref="IOptions{TOptions}"/> for <typeparamref name="TSettings" /> and validates any annotations.
    /// </summary>
    public static HostApplicationBuilder AddSettings<TSettings>(this HostApplicationBuilder builder)
        where TSettings : class
    {
        builder.Services.AddRequiredSettings<TSettings>();
        return builder;
    }

    public static TSettings GetRequiredSettings<TSettings>(this HostApplicationBuilder builder)
        where TSettings : class
    {
        return builder.Configuration.GetRequiredSettings<TSettings>();
    }
}
