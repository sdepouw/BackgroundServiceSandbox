using Core7Library;
using Core7Library.CatFacts;
using Core7Library.TypedHttpClient;
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
    services.AddRefitClient<ICatFactsClient>()
        .ConfigureHttpClient(c =>
        {
            var catFactsClientSettings = hostBuilderContext.Configuration.GetSection(nameof(MySettings))
                .Get<MySettings>()!.CatFactsClientSettings;
            c.BaseAddress = new Uri(catFactsClientSettings.Host);
        });

    services.AddTransient<ITypedHttpClientFactory<ICatFactsClient>, TypedHttpClientFactory>();
});

IHost host = builder.Build();
host.Run();
