using Core7Library.Extensions;
using Microsoft.Extensions.Hosting;
using Refit;

namespace Core7Library.BearerTokenStuff;

/// <summary>
/// Static class that returns bearer tokens. Used when calling <see cref="ServiceCollectionExtensions.AddRefitClient{TClientInterface}"/>
/// when setting <see cref="RefitSettings.AuthorizationHeaderValueGetter"/> (i.e. passing "useAuthHeaderGetter" as true).
/// After building the <see cref="IHost"/>, call <see cref="SetBearerTokenGetterFunc" /> so this factory knows how to get
/// the bearer token. If that doesn't happen, then <see cref="InvalidOperationException"/> will be thrown.
/// </summary>
public static class AuthorizationBearerTokenFactory
{
    private static Func<CancellationToken, Task<string>>? _getBearerTokenAsyncFunc;

    /// <summary>
    /// Returns bearer token string to be added to Authorization headers.
    /// Use the returned token like so: "Authorization: Bearer {token}"
    /// </summary>
    /// <returns>Token string to use in Authorization Bearer header</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="SetBearerTokenGetterFunc"/> not called prior to calling this method
    /// </exception>
    public static Task<string> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        VerifyBearerTokenGetterFuncIsSet();
        return _getBearerTokenAsyncFunc!(cancellationToken);
    }

    public static void VerifyBearerTokenGetterFuncIsSet()
    {
        if (_getBearerTokenAsyncFunc is null)
        {
            throw new InvalidOperationException(
                $"Cannot call {nameof(AuthorizationBearerTokenFactory)}.{nameof(GetBearerTokenAsync)} without calling {nameof(AuthorizationBearerTokenFactory)}.{nameof(SetBearerTokenGetterFunc)} first!");
        }
    }

    public static void SetBearerTokenGetterFunc(Func<CancellationToken, Task<string>> getBearerTokenAsyncFunc)
        => _getBearerTokenAsyncFunc = getBearerTokenAsyncFunc;
}
