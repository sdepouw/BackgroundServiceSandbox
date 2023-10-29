using Core7Library.CatFacts;
using Core7Library.Extensions;
using Refit;
using WorkServiceSeven;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .AddBaseSettings<MySettings>()
    .ConfigureServices((hostBuilderContext, services) =>
    {
        services.AddHostedService<Worker7>();
        var clientSettings = hostBuilderContext.GetRequiredSettings<MySettings>().CatFactsClientSettings;
        services.AddRefitClient<ICatFactsClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(clientSettings.Host))
            .SetHandlerLifetime(TimeSpan.FromMinutes(10));
    });

IHost host = builder.Build();
host.Run();
