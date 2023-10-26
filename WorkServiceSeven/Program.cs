using Core7Library;
using Core7Library.CatFacts;
using Core7Library.Extensions;
using Refit;
using WorkServiceSeven;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((hostBuilderContext, services) =>
{
    services.AddHostedService<Worker7>();
    services.AddOptions<MySettings>()
        .BindConfiguration(nameof(MySettings))
        .ValidateDataAnnotations()
        .ValidateOnStart();
    var catFactsClientSettings = hostBuilderContext.Configuration.GetRequiredConfig<MySettings>().CatFactsClientSettings;
    services.AddRefitClient<ICatFactsClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(catFactsClientSettings.Host))
        .SetHandlerLifetime(TimeSpan.FromMinutes(10));
});

IHost host = builder.Build();
host.Run();
