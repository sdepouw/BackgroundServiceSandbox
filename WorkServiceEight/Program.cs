using Core7Library.CatFacts;
using Core7Library.Extensions;
using WorkServiceEight;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker8>();
builder.AddSettings<MySettings>();
var clientSettings = builder.GetRequiredSettings<MySettings>().CatFactsClientSettings;
builder.Services.AddRefitClient<ICatFactsClient>(c => c.BaseAddress = new Uri(clientSettings.Host));

IHost host = builder.Build();
host.Run();
