using Core7Library.CatFacts;
using Core7Library.Extensions;
using Microsoft.Extensions.Options;
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

public interface IBearerTokenFactory
{
    Task<string> GetBearerTokenAsync(CancellationToken cancellationToken);
}

public class BearerTokenFactory : IBearerTokenFactory
{
    private static string _cachedToken = "";
    private static DateTime _cacheExpiration;
    private static readonly SemaphoreSlim Semaphore = new(1);

    private readonly IOAuthClient _client;
    private bool CachedTokenIsExpired()
    {
        var isExpired = string.IsNullOrWhiteSpace(_cachedToken) || DateTime.UtcNow > _cacheExpiration;
        return isExpired;
    }

    public BearerTokenFactory(IOAuthClient client, IOptions<MySettings> settings)
    {
        _client = client;
    }

    // Probably shouldn't cache? From what I'm reading seems that one shouldn't.
    // May just keep the second client that gets the bearer token and loads it up.
    public async Task<string> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        if (!CachedTokenIsExpired()) return _cachedToken;

        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            // Check again if it was set while waiting.
            if (!CachedTokenIsExpired()) return _cachedToken;
            var response = await _client.GetBearerTokenAsync(cancellationToken);
            _cachedToken = response.Token;
            _cacheExpiration = DateTime.UtcNow.AddSeconds(response.SecondsUntilExpiration);
        }
        finally
        {
            Semaphore.Release();
        }
        return _cachedToken;
    }
}

public class AuthToken
{
    public string Token { get; set; } = "";
    public int SecondsUntilExpiration { get; set; }
}

public interface IOAuthClient
{
    [Post("/oauth")]
    public Task<AuthToken> GetBearerTokenAsync(CancellationToken cancellationToken);
}

public class FakeOAuthClient : IOAuthClient
{
    public Task<AuthToken> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(new AuthToken
        {
            SecondsUntilExpiration = 30,
            Token = $"Foo-{DateTime.UtcNow:s}"
        });
    }
}
