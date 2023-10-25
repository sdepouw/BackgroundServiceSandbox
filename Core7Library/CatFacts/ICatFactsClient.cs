using Core7Library.TypedHttpClient;
using Refit;

namespace Core7Library.CatFacts;

public interface ICatFactsClient : ITypedHttpClient
{
    [Get("/facts/")]
    Task<List<CatFact>> GetTheFacts();
}
