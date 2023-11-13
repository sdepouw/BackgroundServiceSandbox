using Core7Library.BearerTokenStuff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace Core7Library.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up <see cref="IOptions{TOptions}"/> for <typeparamref name="TSettings" /> and validates any annotations.
    /// </summary>
    public static OptionsBuilder<TSettings> AddRequiredSettings<TSettings>(this IServiceCollection services)
        where TSettings : class
    {
        return services.AddOptions<TSettings>()
            .BindConfiguration(typeof(TSettings).Name)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    /// <summary>
    /// Configures <typeparamref name="TClientInterface"/> as a Refit client, setting other conventional defaults.
    /// </summary>
    /// <param name="services">The service collection being built</param>
    /// <param name="configureHttpClient">Action to take when configuring the <see cref="HttpClient"/></param>
    /// <param name="useAuthHeaderGetter">When true, will configure Refit to attempt to fetch a bearer token from <see cref="IBearerTokenFactory"/></param>
    /// <param name="enableRequestResponseLogging">When true, will log every single HTTP request and response using <see cref="HttpLoggingHandler"/></param>
    /// <param name="disableAutoRedirect">
    /// When true, will attempt to disable the ability for this client to do redirects when making HTTP calls.
    /// Typically, one might do this to prevent redirects to login pages when trying to use an API.
    /// </param>
    /// <param name="handlerLifetimeInMinutes">
    /// How long the handler should live for before it is renewed. In long-running applications and services,
    /// set this to a low value because they're meant to be short-lived. Default is 10 minutes.
    /// </param>
    /// <typeparam name="TClientInterface">A Refit client interface.</typeparam>
    /// <returns>The builder used to build this client, to allow for further customization if needed.</returns>
    public static IHttpClientBuilder AddRefitClient<TClientInterface>(this IServiceCollection services,
        Action<HttpClient> configureHttpClient, bool useAuthHeaderGetter = false, bool enableRequestResponseLogging = false,
        bool disableAutoRedirect = false, int handlerLifetimeInMinutes = 10)
        where TClientInterface : class
    {
        RefitSettings? refitSettings = null;
        if (useAuthHeaderGetter)
        {
            AuthorizationBearerTokenFactory.Enable();
            refitSettings = new RefitSettings
            {
                AuthorizationHeaderValueGetter = (_, cancellationToken) => AuthorizationBearerTokenFactory.GetBearerTokenAsync(cancellationToken)
            };
        }
        var builder = services.AddRefitClient<TClientInterface>(refitSettings);
        if (disableAutoRedirect) builder = builder.DisableAutoRedirect();
        builder
            .ConfigureHttpClient(configureHttpClient)
            .SetHandlerLifetime(TimeSpan.FromMinutes(handlerLifetimeInMinutes));
        if (enableRequestResponseLogging)
        {
            // Cannot use builder.AddHttpMessageHandler<HttpLoggingHandler>, because Refit and DI mingling throws an exception.
            // See https://github.com/reactiveui/refit/issues/1403#issuecomment-1499380557 for workaround source
            builder.AddHttpMessageHandler(svc => new HttpLoggingHandler(svc.GetRequiredService<ILogger<HttpLoggingHandler>>()));
            services.AddSingleton<HttpLoggingHandler>(); // Calling this multiple times seems to be fine.
        }
        return builder;
    }
}
