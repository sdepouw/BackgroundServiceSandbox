using Refit;

namespace Core7Library;

public interface ICatFactsClient
{
    [Get("/facts/")]
    Task<List<CatFact>> GetTheFacts();
}
