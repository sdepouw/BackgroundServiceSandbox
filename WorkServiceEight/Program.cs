using Core7Library;
using Core7Library.BearerTokenStuff;
using Core7Library.CatFacts;
using Core7Library.Extensions;
using Serilog;
using WorkServiceEight;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.AddSettings<MySettings>();
builder.AddSettings<CatFactsClientSettings>();
builder.Services.AddHostedService<Worker8>();
builder.Services.AddSerilog(config => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddTransient<IBearerTokenFactory, BearerTokenFactory>();
builder.Services.AddTransient<IOAuthClientService, OAuthClientService>();
builder.Services.AddTransient<ICatFactsClientService, CatFactsClientService>();

var catFactsSettings = builder.GetRequiredSettings<CatFactsClientSettings>();
builder.Services.AddRefitClient<IOAuthClient>(c => c.BaseAddress = new Uri("https://example.com/"));
builder.Services.AddRefitClient<ICatFactsClient>(c => c.BaseAddress = new Uri(catFactsSettings.Host), useAuthHeaderGetter: true, enableRequestResponseLogging: true);

IHost host = builder.Build();
ServiceCollectionExtensions.Provider = host.Services;
host.Run();
