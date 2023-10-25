using Core7Library;
using Core7Library.CatFacts;
using Core7Library.Extensions;
using WorkServiceSeven;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((hostBuilderContext, services) =>
{
    services.AddHostedService<Worker7>();
    services.AddOptions<MySettings>()
        .BindConfiguration(nameof(MySettings))
        .ValidateDataAnnotations()
        .ValidateOnStart();
    var catFactsClientSettings = hostBuilderContext.Configuration.GetRequiredConfig<MySettings>().CatFactsClientSettings;
    services.AddRefitHttpClientAndFactory<ICatFactsClient>(catFactsClientSettings.Host);
});

IHost host = builder.Build();
host.Run();
