namespace Core7Library.BearerTokenStuff;

public class BearerTokenFactory : IBearerTokenFactory
{
    private static string _cachedToken = "";
    private static DateTime _cacheExpiration;
    private static readonly SemaphoreSlim Semaphore = new(1);

    private readonly IOAuthClientService _oAuthClientService;
    private bool CachedTokenIsExpired()
    {
        var isExpired = string.IsNullOrWhiteSpace(_cachedToken) || DateTime.UtcNow > _cacheExpiration;
        return isExpired;
    }

    public BearerTokenFactory(IOAuthClientService oAuthClientService)
    {
        _oAuthClientService = oAuthClientService;
    }

    // Probably shouldn't cache? From what I'm reading seems that one shouldn't.
    // May just keep the second client that gets the bearer token and loads it up.
    public async Task<string> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        if (!CachedTokenIsExpired()) return _cachedToken;

        await Semaphore.WaitAsync(cancellationToken);
        try
        {
            // Check again if it was set while waiting.
            if (!CachedTokenIsExpired()) return _cachedToken;
            var response = await _oAuthClientService.GetBearerTokenAsync(cancellationToken);
            _cachedToken = response.Token;
            _cacheExpiration = DateTime.UtcNow.AddSeconds(response.SecondsUntilExpiration);
        }
        finally
        {
            Semaphore.Release();
        }
        return _cachedToken;
    }
}
