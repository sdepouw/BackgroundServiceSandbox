using Core7Library.CatFacts;
using Microsoft.Extensions.Logging;

namespace Core7Library.BearerTokenStuff;

public class BearerTokenFactory : ClientServiceBase, IBearerTokenFactory
{
    private readonly IOAuthClient _client;

    public BearerTokenFactory(ILogger<BearerTokenFactory> logger, IOAuthClient client) : base(logger)
    {
        _client = client;
    }

    public async Task<string> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        AuthToken? response = await MakeRequestAsync(() => _client.GetBearerTokenAsync(cancellationToken), new AuthToken());
        // Any caching or other logic regarding the full token would be implemented here.
        return response?.Token ?? string.Empty;
    }
}
