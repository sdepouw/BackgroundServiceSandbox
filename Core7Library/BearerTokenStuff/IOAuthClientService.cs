namespace Core7Library.BearerTokenStuff;

public interface IOAuthClientService
{
    Task<string> GetBearerTokenAsync(CancellationToken cancellationToken);
}