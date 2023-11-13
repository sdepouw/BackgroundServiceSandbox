using Core7Library.CatFacts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core7Library;

public class CatFactsClientService : RefitClientServiceBase<ICatFactsClient>, ICatFactsClientService
{
    private readonly CatFactsClientSettings _settings;

    public CatFactsClientService(ICatFactsClient catFactsClient, ILogger<CatFactsClientService> logger, IOptions<CatFactsClientSettings> settings)
        : base (catFactsClient, logger)
    {
        _settings = settings.Value;
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

        return GetApiResponse(RefitClient.GetTheFactsAsync(_settings.GetTheFactsRoute, cancellationToken));
    }

    public Task<CatFact?> Explode(CancellationToken cancellationToken)
    {
        return GetApiResponse(RefitClient.GetSingleFact("Whatever", new(), cancellationToken));
    }
}
