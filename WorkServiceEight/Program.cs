using Core7Library;
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
        // TODO: Is there a cleaner way to access application settings at this point?
        var catFactsClientSettings = builder.Configuration.GetSection(nameof(MySettings))
            .Get<MySettings>()!.CatFactsClientSettings;
        c.BaseAddress = new Uri(catFactsClientSettings.Host);
    });

builder.Services.AddTransient<ICatFactsClientFactory, CatFactsClientFactory>();

IHost host = builder.Build();
host.Run();
