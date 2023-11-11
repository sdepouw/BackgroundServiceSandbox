using Refit;

namespace Core7Library.BearerTokenStuff;

public interface IOAuthClient
{
    [Post("/oauth")]
    public Task<AuthToken> GetBearerTokenAsync(CancellationToken cancellationToken);
}
