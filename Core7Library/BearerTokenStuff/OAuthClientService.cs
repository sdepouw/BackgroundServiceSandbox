using Core7Library.CatFacts;
using Microsoft.Extensions.Logging;

namespace Core7Library.BearerTokenStuff;

public class OAuthClientService : RefitClientServiceBase<IOAuthClient>, IOAuthClientService
{
    public OAuthClientService(IOAuthClient client, ILogger<OAuthClientService> logger) : base(client, logger) { }

    public async Task<string> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        AuthToken response = await GetApiResponse(RefitClient.GetBearerTokenAsync(cancellationToken), new AuthToken());
        // Any caching or other logic regarding the full token would be implemented here.
        return response.Token;
    }
}
