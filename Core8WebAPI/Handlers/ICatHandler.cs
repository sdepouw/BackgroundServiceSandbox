using Core7Library.CatFacts;

namespace Core8WebAPI.Handlers;

public interface ICatHandler
{
    Task<List<CatFact>> HandleAsync(CancellationToken token);
}
