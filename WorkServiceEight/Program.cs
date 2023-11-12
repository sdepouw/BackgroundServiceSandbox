using Core7Library;
using Core7Library.BearerTokenStuff;
using Core7Library.CatFacts;
using Core7Library.Extensions;
using Refit;
using Serilog;
using WorkServiceEight;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker8>();
builder.AddSettings<MySettings>();
builder.AddSettings<CatFactsClientSettings>();
builder.Services.AddSerilog(config => config.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddTransient<IBearerTokenFactory, BearerTokenFactory>();
builder.Services.AddTransient<IOAuthClient, FakeOAuthClient>();
builder.Services.AddTransient<ICatFactsService, CatFactsClientService>();
IServiceProvider providerWithClient = builder.Services.BuildServiceProvider();
var refitSettings = new RefitSettings
{
    AuthorizationHeaderValueGetter = (_, cancellationToken) =>
    {
        var client = providerWithClient.GetRequiredService<IBearerTokenFactory>();
        return client.GetBearerTokenAsync(cancellationToken);
    }
};
var clientSettings = builder.GetRequiredSettings<CatFactsClientSettings>();
builder.Services.AddRefitClient<ICatFactsClient>(c => c.BaseAddress = new Uri(clientSettings.Host), refitSettings, true);

IHost host = builder.Build();
host.Run();
