using Core7Library.CatFacts;
using Core7Library.Extensions;
using Refit;
using WorkServiceSeven;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder
    .AddBaseSettings<MySettings>()
    .ConfigureServices((hostBuilderContext, services) =>
{
    services.AddHostedService<Worker7>();
    MySettings requiredConfig = hostBuilderContext.Configuration.GetRequiredConfig<MySettings>();
    var catFactsClientSettings = requiredConfig.CatFactsClientSettings;
    services.AddRefitClient<ICatFactsClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(catFactsClientSettings.Host))
        .SetHandlerLifetime(TimeSpan.FromMinutes(10));
});

IHost host = builder.Build();
host.Run();
