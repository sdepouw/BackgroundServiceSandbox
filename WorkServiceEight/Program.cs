using Core7Library.CatFacts;
using Core7Library.Extensions;
using Refit;
using WorkServiceEight;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker8>();
builder.AddBaseSettings<MySettings>();
var clientSettings = builder.GetRequiredSettings<MySettings>().CatFactsClientSettings;
builder.Services.AddRefitClient<ICatFactsClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(clientSettings.Host))
    .SetHandlerLifetime(TimeSpan.FromMinutes(10));

IHost host = builder.Build();
host.Run();
