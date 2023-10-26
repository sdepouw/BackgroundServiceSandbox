using Core7Library;
using Core7Library.CatFacts;
using Core7Library.Extensions;
using Refit;
using WorkServiceEight;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker8>();
builder.Services.AddOptions<MySettings>()
    .BindConfiguration(nameof(MySettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var catFactsClientSettings = builder.Configuration.GetRequiredConfig<MySettings>().CatFactsClientSettings;
builder.Services.AddRefitClient<ICatFactsClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(catFactsClientSettings.Host))
    .SetHandlerLifetime(TimeSpan.FromMinutes(10));

IHost host = builder.Build();
host.Run();
