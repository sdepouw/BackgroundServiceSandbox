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

    public Task<AuthToken> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        return MakeRequestAsync(() => _client.GetBearerTokenAsync(cancellationToken), new AuthToken())!;
    }
}