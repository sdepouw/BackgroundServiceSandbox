using Core7Library.CatFacts;

namespace Core8WebAPI.Handlers;

public class CatHandler(ILogger<CatHandler> logger, ICatFactsClientService catFactsService) : ICatHandler
{
    public async Task<List<CatFact>> HandleAsync(CancellationToken token)
    {
        logger.LogInformation("Getting some cat facts!");
        return await catFactsService.GetTheFactsAsync(token);
    }
}
