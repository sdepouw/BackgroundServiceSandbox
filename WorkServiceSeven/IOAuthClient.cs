using Refit;

public interface IOAuthClient
{
    [Post("/oauth")]
    public Task<AuthToken> GetBearerTokenAsync(CancellationToken cancellationToken);
}