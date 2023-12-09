using Autofac;
using Autofac.Extensions.DependencyInjection;
using Core7Library;
using Core7Library.BearerTokenStuff;
using Core7Library.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;
using Serilog;

namespace Core8Library.SuperBuilder;

public class SuperHostBuilder
{
    private readonly IHostBuilderWrapper _builder;

    private readonly List<Type> _settingsTypes = new();
    private Func<IHost, CancellationToken, Task<string>>? _getBearerTokenAsyncFunc = null;

    private SuperHostBuilder(IHostBuilderWrapper builder) => _builder = builder;

    /// <summary>
    /// Creates a new <see cref="WebApplicationBuilder"/>, with Serilog logging and dependencies registered with Autofac
    /// </summary>
    /// <param name="autofacModulesToRegister">Any custom Autofac modules that should also be registered</param>
    public static SuperHostBuilder CreateWebApp(params Module[] autofacModulesToRegister)
        => CreateWebAppManually().WithLogging().WithDependenciesRegistered(autofacModulesToRegister);

    /// <summary>
    /// Only call this if you want to configure logging and DI manually.
    /// Normally, always call <see cref="CreateWebApp"/>.
    /// </summary>
    public static SuperHostBuilder CreateWebAppManually() => new(new WebApplicationBuilderWrapper());

    /// <summary>
    /// Creates a new <see cref="SuperHostBuilder"/> configured to run as a Windows service,
    /// with Serilog logging and dependencies registered with Autofac
    /// </summary>
    /// <param name="autofacModulesToRegister">Any custom Autofac modules that should also be registered</param>
    /// <typeparam name="THostedService">The worker service class to register</typeparam>
    public static SuperHostBuilder CreateHostApp<THostedService>(params Module[] autofacModulesToRegister)
        where THostedService : class, IHostedService => CreateHostAppManually<THostedService>()
            .WithLogging()
            .WithDependenciesRegistered(autofacModulesToRegister);

    /// <summary>
    /// Only call this if you want to configure logging and DI manually.
    /// Still registers the app as a hosted service.
    /// Normally, always call <see cref="CreateHostApp{THostedService}"/>.
    /// </summary>
    public static SuperHostBuilder CreateHostAppManually<THostedService>() where THostedService : class, IHostedService
        => new SuperHostBuilder(new HostApplicationBuilderWrapper())
        .WithHostedWindowsService<THostedService>();

    /// <summary>
    /// Creates an <see cref="IHost" /> and performs validation on configured settings (DataAnnotations, connection strings)
    /// and the configured logger (make sure it can write to file if configured to do so)
    /// </summary>
    /// <returns>An <see cref="IHost"/> that can be run</returns>
    /// <exception cref="ApplicationException">Thrown when a validation error occurs</exception>
    public IHost BuildAndValidate()
    {
        IHost host = _builder.Build();
        if (_getBearerTokenAsyncFunc is not null)
        {
            AuthBearerTokenFactory.SetBearerTokenGetterFunc(token => _getBearerTokenAsyncFunc(host, token));
        }
        HostValidator.Validate(host, _settingsTypes);
        return host;
    }

    /// <summary>
    /// Configures settings for a particular type, setting up DataAnnotations validation
    /// for them and making them available throughout the application by injecting
    /// <see cref="IOptions{TOptions}"/> into a constructor
    /// </summary>
    /// <returns>
    /// A <typeparamref name="TSettings"/> instance hydrated with settings from configuration.
    /// These are not validated, but are necessary for use when configuring the service on startup.
    /// </returns>
    public TSettings WithSettings<TSettings>()
        where TSettings : class
    {
        _builder.Services.AddRequiredSettings<TSettings>();
        _settingsTypes.Add(typeof(TSettings));
        return _builder.Configuration.GetRequiredSettings<TSettings>();
    }

    /// <summary>
    /// Registers dependencies for this library as well as for the calling service's
    /// </summary>
    /// <param name="autofacModulesToRegister">Any custom Autofac modules that should also be registered</param>
    public SuperHostBuilder WithDependenciesRegistered(Module[] autofacModulesToRegister)
    {
        void ConfigureAutofac(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule(new SuperAutofacModule());
            foreach (var autofacModule in autofacModulesToRegister)
            {
                containerBuilder.RegisterModule(autofacModule);
            }
        }
        _builder.RegisterDependencies(new AutofacServiceProviderFactory(), ConfigureAutofac);
        return this;
    }

    /// <summary>
    /// Adds Serilog logging, configured from file
    /// </summary>
    public SuperHostBuilder WithLogging()
    {
        _builder.Services.AddSerilog(config => config.ReadFrom.Configuration(_builder.Configuration));
        return this;
    }

        /// <summary>
    /// Configures <typeparamref name="TClientInterface"/> as a Refit client, setting other conventional defaults.
    /// </summary>
    /// <param name="configureHttpClient">Action to take when configuring the <see cref="HttpClient"/></param>
    /// <param name="getBearerTokenAsyncFunc">When provided, will configure Refit to attempt to fetch a bearer token from <see cref="AuthBearerTokenFactory"/></param>
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
    public SuperHostBuilder WithRefitClient<TClientInterface>(Action<HttpClient> configureHttpClient, Func<IHost, CancellationToken, Task<string>>? getBearerTokenAsyncFunc = null, bool enableRequestResponseLogging = false,
        bool disableAutoRedirect = false, int handlerLifetimeInMinutes = 10)
        where TClientInterface : class
    {
        RefitSettings? refitSettings = null;
        if (getBearerTokenAsyncFunc is not null)
        {
            _getBearerTokenAsyncFunc = getBearerTokenAsyncFunc;
            refitSettings = new RefitSettings
            {
                AuthorizationHeaderValueGetter = (_, cancellationToken) => AuthBearerTokenFactory.GetBearerTokenAsync(cancellationToken)
            };
        }
        IHttpClientBuilder builder = _builder.Services.AddRefitClient<TClientInterface>(refitSettings);
        if (disableAutoRedirect) builder = builder.DisableAutoRedirect();
        builder
            .ConfigureHttpClient(configureHttpClient)
            .SetHandlerLifetime(TimeSpan.FromMinutes(handlerLifetimeInMinutes));
        if (enableRequestResponseLogging)
        {
            // Cannot use builder.AddHttpMessageHandler<HttpLoggingHandler>, because Refit and DI mingling throws an exception.
            // See https://github.com/reactiveui/refit/issues/1403#issuecomment-1499380557 for workaround source
            builder.AddHttpMessageHandler(svc => new HttpLoggingHandler(svc.GetRequiredService<ILogger<HttpLoggingHandler>>()));
            builder.Services.AddSingleton<HttpLoggingHandler>(); // Calling this multiple times seems to be fine.
        }
        return this;
    }

    /// <summary>
    /// Adds the specified serviced as a Hosted Service and Windows Service
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when building a web application instead of a hosted one</exception>
    private SuperHostBuilder WithHostedWindowsService<THostedService>()
        where THostedService : class, IHostedService
    {
        if (_builder is WebApplicationBuilderWrapper)
        {
            throw new InvalidOperationException("Cannot add web apps as hosted Windows services. Only host apps.");
        }
        _builder.Services.AddHostedService<THostedService>();
        _builder.Services.AddWindowsService();
        return this;
    }
}
