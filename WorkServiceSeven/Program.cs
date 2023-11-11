using Core7Library.CatFacts;
using Core7Library.Extensions;
using Refit;
using WorkServiceSeven;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .AddSettings<MySettings>()
    .ConfigureServices((hostBuilderContext, services) =>
    {

        services.AddTransient<IBearerTokenFactory, BearerTokenFactory>();
        services.AddTransient<IOAuthClient, FakeOAuthClient>();
        services.AddTransient<ICatFactsService, CatFactsClientService>();
        // services.AddRefitClient<IOAuthClient>(c => c.BaseAddress = new Uri("https://example.com/"));

        var clientSettings = hostBuilderContext.GetRequiredSettings<MySettings>().CatFactsClientSettings;
        services.AddHostedService<Worker7>();
        IServiceProvider providerWithClient = services.BuildServiceProvider();
        var refitSettings = new RefitSettings
        {
            AuthorizationHeaderValueGetter = (_, cancellationToken) =>
            {
                var client = providerWithClient.GetRequiredService<IBearerTokenFactory>();
                return client.GetBearerTokenAsync(cancellationToken);
            }
        };
        services.AddRefitClient<ICatFactsClient>(c => c.BaseAddress = new Uri(clientSettings.Host), refitSettings, true);
    });

IHost host = builder.Build();
host.Run();
