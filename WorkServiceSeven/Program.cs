using Core7Library;
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
            // TODO: Is there a cleaner way to access application settings at this point?
            var catFactsClientSettings = hostBuilderContext.Configuration.GetSection(nameof(MySettings))
                .Get<MySettings>()!.CatFactsClientSettings;
            c.BaseAddress = new Uri(catFactsClientSettings.Host);
        });

    services.AddTransient<ICatFactsClientFactory, CatFactsClientFactory>();
});

IHost host = builder.Build();
host.Run();
