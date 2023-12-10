using Core7Library.BearerTokenStuff;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core7Library.CatFacts;

public class CatFactsClientService : RefitClientServiceBase<ICatFactsClient>, ICatFactsClientService
{
    private readonly CatFactsClientSettings _settings;

    public CatFactsClientService(ICatFactsClient catFactsClient, ILogger<CatFactsClientService> logger, IOptions<CatFactsClientSettings> settings)
        : base (catFactsClient, logger)
    {
        _settings = settings.Value;
    }

    public async Task<string> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        AuthToken response = await GetApiResponse(RefitClient.GetBearerTokenAsync(cancellationToken), new AuthToken { SecondsUntilExpiration = 2303, Token = "default-token-value"});
        // Any caching or other logic regarding the full token would be implemented here.
        return response.Token;
    }

    public Task<List<CatFact>> GetTheFactsAsync(CancellationToken cancellationToken)
    {
        // There a way to hit an API endpoint that returns ApiResponse?
        // Func<Task> taskWithNoReturn = () =>
        // {
        //     _catFactsClient.GetTheFactsAsync("FoobarThisWillNotWork", cancellationToken);
        //     return Task.CompletedTask;
        // };
        //MakeRequestAsync(taskWithNoReturn);
        // Func<Task<ApiResponse<string?>>> taskWithSimpleReturn = () => Task.FromResult(new ApiResponse<string?>(null!, "foo", new()));
        // MakeRequestAsync(taskWithSimpleReturn);

        return GetApiResponse(RefitClient.GetTheFactsAsync(_settings.GetTheFactsRoute, cancellationToken), new List<CatFact>());
    }

    public Task<CatFact?> Explode(CancellationToken cancellationToken)
    {
        return GetApiResponse(RefitClient.GetSingleFact("Whatever", new(), cancellationToken));
    }
}
