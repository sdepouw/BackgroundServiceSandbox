using Core7Library.CatFacts;
using Microsoft.Extensions.Logging;

namespace Core7Library.BearerTokenStuff;

public class OAuthClientService : ClientServiceBase, IOAuthClientService
{
    private readonly IOAuthClient _client;

    public OAuthClientService(ILogger<OAuthClientService> logger, IOAuthClient client) : base(logger)
    {
        _client = client;
    }

    public async Task<string> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        AuthToken response = await GetApiResponse(_client.GetBearerTokenAsync(cancellationToken), new AuthToken());
        // Any caching or other logic regarding the full token would be implemented here.
        return response.Token;
    }
}
