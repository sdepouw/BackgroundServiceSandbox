using Refit;

namespace Core7Library.CatFacts;

public interface ICatFactsClient
{
    [Get("/facts/")]
    Task<List<CatFact>> GetTheFacts();
}
