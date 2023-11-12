namespace Core7Library.BearerTokenStuff;

public interface IOAuthClientService
{
    Task<AuthToken> GetBearerTokenAsync(CancellationToken cancellationToken);
}