using Refit;

namespace Core7Library.CatFacts;

[Headers("Authorization: Gobbledeegook")]
public interface ICatFactsClient
{
    [Get("/facts/")]
    Task<List<CatFact>> GetTheFacts();
}
