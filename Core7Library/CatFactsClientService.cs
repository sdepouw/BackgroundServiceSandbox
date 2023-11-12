using Core7Library.CatFacts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core7Library;

public class CatFactsClientService : ClientServiceBase, ICatFactsClientService
{
    private readonly CatFactsClientSettings _settings;
    private readonly ICatFactsClient _catFactsClient;

    public CatFactsClientService(IOptions<CatFactsClientSettings> settings, ILogger<CatFactsClientService> logger, ICatFactsClient catFactsClient)
        : base (logger)
    {
        _settings = settings.Value;
        _catFactsClient = catFactsClient;
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

        return MakeRequestAsync(_catFactsClient.GetTheFactsAsync(_settings.GetTheFactsRoute, cancellationToken));
    }

    public Task<CatFact?> Explode(CancellationToken cancellationToken)
    {
        return MakeRequestAsync(_catFactsClient.GetSingleFact("Whatever", new(), cancellationToken));
    }
}
