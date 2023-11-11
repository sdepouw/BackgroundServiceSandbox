public interface IBearerTokenFactory
{
    Task<string> GetBearerTokenAsync(CancellationToken cancellationToken);
}