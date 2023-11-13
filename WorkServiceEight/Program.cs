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

builder.Services.AddTransient<IOAuthClientService, OAuthClientService>();
builder.Services.AddTransient<ICatFactsClientService, CatFactsClientService>();

var mySettings = builder.GetRequiredSettings<MySettings>();
var catFactsSettings = builder.GetRequiredSettings<CatFactsClientSettings>();
builder.Services.AddRefitClient<IOAuthClient>(c => c.BaseAddress = new Uri("https://example.com/"), enableRequestResponseLogging: mySettings.EnableHttpRequestResponseLogging);
builder.Services.AddRefitClient<ICatFactsClient>(c => c.BaseAddress = new Uri(catFactsSettings.Host), useAuthHeaderGetter: true, enableRequestResponseLogging: mySettings.EnableHttpRequestResponseLogging);

IHost host = builder.Build();
Task<string> GetBearerTokenAsyncFunc(CancellationToken cancellationToken) => host.Services.GetRequiredService<IOAuthClientService>().GetBearerTokenAsync(cancellationToken);
BearerTokenFactory.SetBearerTokenGetterFunc(GetBearerTokenAsyncFunc);
host.Run();
