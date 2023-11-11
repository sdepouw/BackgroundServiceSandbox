namespace Core7Library.BearerTokenStuff;

public interface IBearerTokenFactory
{
    Task<string> GetBearerTokenAsync(CancellationToken cancellationToken);
}
