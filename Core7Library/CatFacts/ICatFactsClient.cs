using Refit;

namespace Core7Library.CatFacts;

public interface ICatFactsClient
{
    /// <summary>
    /// Refit notes:
    /// - Always return <see cref="ApiResponse{T}"/> so that status code can be checked.
    /// - Return type should always be marked as nullable (failed requests return null)
    /// - Can make route Settings-injection capable by passing a string in for the whole route, or part of it
    /// - Last parameter can be cancellation token and it'll work as expected
    /// </summary>
    /// <param name="theRoute">The route to call. Injected instead of hardcoded so that it can be configured</param>
    /// <param name="cancellationToken">CancellationToken to respect.</param>
    /// <returns></returns>
    [Get("/{theRoute}")]
    Task<ApiResponse<List<CatFact>?>> GetTheFactsAsync(string theRoute, CancellationToken cancellationToken);
}
