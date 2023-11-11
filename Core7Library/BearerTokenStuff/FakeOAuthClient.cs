namespace Core7Library.BearerTokenStuff;

public class FakeOAuthClient : IOAuthClient
{
    public Task<AuthToken> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(new AuthToken
        {
            SecondsUntilExpiration = 30,
            Token = $"Foo-{DateTime.UtcNow:s}"
        });
    }
}
