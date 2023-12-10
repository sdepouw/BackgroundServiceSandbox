namespace Core7Library.CatFacts;

public interface ICatFactsClientService
{
    Task<string> GetBearerTokenAsync(CancellationToken cancellationToken);
    Task<List<CatFact>> GetTheFactsAsync(CancellationToken cancellationToken);
    /// <summary>
    /// This is not a real thing. It'll just 404.
    /// </summary>
    /// <returns>Nothing!</returns>
    Task<CatFact?> Explode(CancellationToken cancellationToken);
}
