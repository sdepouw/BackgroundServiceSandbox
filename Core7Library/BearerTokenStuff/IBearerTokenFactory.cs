using Core7Library.Extensions;

namespace Core7Library.BearerTokenStuff;

/// <summary>
/// Implement when using <see cref="ServiceCollectionExtensions.AddRefitClient{TClientInterface}"/> and
/// setting "useAuthHeaderGetter" to true. That will use the implementation of this interface to get
/// bearer tokens.
/// </summary>
public interface IBearerTokenFactory
{
    /// <summary>
    /// Returns bearer token string to be added to Authorization headers.
    /// Use: "Authorization: Bearer {token}"
    /// </summary>
    /// <returns>Token string to use in Authorization Bearer header</returns>
    Task<string> GetBearerTokenAsync(CancellationToken cancellationToken);
}
