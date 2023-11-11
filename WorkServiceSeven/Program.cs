using System.Diagnostics;
using System.Net.Http.Headers;
using Core7Library.CatFacts;
using Core7Library.Extensions;
using Microsoft.Extensions.Http.Logging;
using Microsoft.Extensions.Options;
using Refit;
using WorkServiceSeven;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .AddSettings<MySettings>()
    .ConfigureServices((hostBuilderContext, services) =>
    {

        services.AddTransient<IBearerTokenFactory, BearerTokenFactory>();
        services.AddTransient<IOAuthClient, FakeClient>();
        services.AddTransient<ICatFactsService, CatFactsClientService>();
        // services.AddRefitClient<IOAuthClient>()
        //     .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://example.com/"));
        IServiceProvider providerWithClient = services.BuildServiceProvider();
        var clientSettings = hostBuilderContext.GetRequiredSettings<MySettings>().CatFactsClientSettings;
        services.AddHostedService<Worker7>();
        services.AddRefitClient<ICatFactsClient>(new()
            {
                AuthorizationHeaderValueGetter = (_, cancellationToken) =>
                {
                    var client = providerWithClient.GetRequiredService<IBearerTokenFactory>();
                    return client.GetBearerTokenAsync(cancellationToken);
                }
            })
            .ConfigureHttpMessageHandlerBuilder(builder =>
            {
                // new HttpClientHandler().AllowAutoRedirect
                DelegatingHandler primary = (DelegatingHandler)builder.PrimaryHandler;
                var inner = (HttpClientHandler)primary.InnerHandler!;
                inner.AllowAutoRedirect = false;
                // primary.AllowAutoRedirect = false;
            })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(clientSettings.Host))
            .SetHandlerLifetime(TimeSpan.FromMinutes(10))
            .AddHttpMessageHandler<HttpLoggingHandler>(); // Adding this doesn't break AuthorizationHeaderValueGetter
        services.AddSingleton<HttpLoggingHandler>();
    });

IHost host = builder.Build();
host.Run();

public class HttpLoggingHandler : DelegatingHandler
{
    private readonly ILogger<HttpLoggingHandler> _logger;


    public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger)
    {
        _logger = logger;
    }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var req = request;
            var id = Guid.NewGuid().ToString();
            var msg = $"[{id} -   Request]";

            _logger.LogInformation($"{msg}========Start==========");
            _logger.LogInformation($"{msg} {req.Method} {req.RequestUri.PathAndQuery} {req.RequestUri.Scheme}/{req.Version}");
            _logger.LogInformation($"{msg} Host: {req.RequestUri.Scheme}://{req.RequestUri.Host}");

            foreach (var header in req.Headers)
                _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (req.Content != null)
            {
                foreach (var header in req.Content.Headers)
                    _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

                if (req.Content is StringContent || IsTextBasedContentType(req.Headers) ||
                    this.IsTextBasedContentType(req.Content.Headers))
                {
                    var result = await req.Content.ReadAsStringAsync();

                    _logger.LogInformation($"{msg} Content:");
                    _logger.LogInformation($"{msg} {result}");
                }
            }

            var start = DateTime.Now;

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var end = DateTime.Now;

            _logger.LogInformation($"{msg} Duration: {end - start}");
            _logger.LogInformation($"{msg}==========End==========");

            msg = $"[{id} - Response]";
            _logger.LogInformation($"{msg}=========Start=========");

            var resp = response;

            _logger.LogInformation(
                $"{msg} {req.RequestUri.Scheme.ToUpper()}/{resp.Version} {(int) resp.StatusCode} {resp.ReasonPhrase}");

            foreach (var header in resp.Headers)
                _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (resp.Content != null)
            {
                foreach (var header in resp.Content.Headers)
                    _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

                if (resp.Content is StringContent || this.IsTextBasedContentType(resp.Headers) ||
                    this.IsTextBasedContentType(resp.Content.Headers))
                {
                    start = DateTime.Now;
                    var result = await resp.Content.ReadAsStringAsync();
                    end = DateTime.Now;

                    _logger.LogInformation($"{msg} Content:");
                    _logger.LogInformation($"{msg} {result}");
                    _logger.LogInformation($"{msg} Duration: {end - start}");
                }
            }

            _logger.LogInformation($"{msg}==========End==========");
            return response;
        }

        readonly string[] types = new[] {"html", "text", "xml", "json", "txt", "x-www-form-urlencoded"};


        bool IsTextBasedContentType(HttpHeaders headers)
        {
            IEnumerable<string> values;
            if (!headers.TryGetValues("Content-Type", out values))
                return false;
            var header = string.Join(" ", values).ToLowerInvariant();

            return types.Any(t => header.Contains(t));
        }
    }

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

public class FakeClient : IOAuthClient
{
    public async Task<AuthToken> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5000, cancellationToken);
        return new AuthToken
        {
            SecondsUntilExpiration = 30,
            Token = $"Foo-{DateTime.UtcNow:s}"
        };
    }
}
