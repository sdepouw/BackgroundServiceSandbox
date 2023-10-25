using Core7Library;
using Core7Library.CatFacts;
using Core7Library.TypedHttpClient;
using Refit;
using WorkServiceEight;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker8>();
builder.Services.AddOptions<MySettings>()
    .BindConfiguration(nameof(MySettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddRefitClient<ICatFactsClient>()
    .ConfigureHttpClient(c =>
    {
        var catFactsClientSettings = builder.Configuration.GetSection(nameof(MySettings))
            .Get<MySettings>()!.CatFactsClientSettings;
        c.BaseAddress = new Uri(catFactsClientSettings.Host);
    });

builder.Services.AddTransient<ITypedHttpClientFactory<ICatFactsClient>, TypedHttpClientFactory>();

IHost host = builder.Build();
host.Run();
