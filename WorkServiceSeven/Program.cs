using Core7Library;
using Core7Library.BearerTokenStuff;
using Core7Library.CatFacts;
using Core7Library.Extensions;
using Serilog;
using WorkServiceSeven;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .AddSettings<MySettings>()
    .AddSettings<CatFactsClientSettings>()
    .ConfigureServices((hostBuilderContext, services) =>
    {
        services.AddHostedService<Worker7>();
        services.AddSerilog(config => config.ReadFrom.Configuration(hostBuilderContext.Configuration));

        services.AddTransient<IBearerTokenFactory, BearerTokenFactory>();
        services.AddTransient<IOAuthClientService, OAuthClientService>();
        services.AddTransient<ICatFactsClientService, CatFactsClientService>();

        var catFactsSettings = hostBuilderContext.GetRequiredSettings<CatFactsClientSettings>();
        services.AddRefitClient<IOAuthClient>(c => c.BaseAddress = new Uri("https://example.com/"), enableRequestResponseLogging: true);
        services.AddRefitClient<ICatFactsClient>(c => c.BaseAddress = new Uri(catFactsSettings.Host), useAuthHeaderGetter: true, enableRequestResponseLogging: true);
    });

IHost host = builder.Build();
ServiceCollectionExtensions.Provider = host.Services;
host.Run();
