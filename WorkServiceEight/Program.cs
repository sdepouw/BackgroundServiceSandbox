using Core7Library;
using Core7Library.CatFacts;
using Core7Library.Extensions;

using WorkServiceEight;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker8>();
builder.Services.AddOptions<MySettings>()
    .BindConfiguration(nameof(MySettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var catFactsClientSettings = builder.Configuration.GetRequiredConfig<MySettings>().CatFactsClientSettings;

builder.Services.AddRefitHttpClientAndFactory<ICatFactsClient>(catFactsClientSettings.Host);

IHost host = builder.Build();
host.Run();
