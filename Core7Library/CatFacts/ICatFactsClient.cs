using Refit;

namespace Core7Library.CatFacts;

public interface ICatFactsClient
{
    [Get("/{theRoute}")]
    Task<ApiResponse<List<CatFact>?>> GetTheFactsAsync(string theRoute, CancellationToken cancellationToken);
}
