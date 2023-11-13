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
    private static bool _enabled;

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
        if (!_enabled) throw new InvalidOperationException($"Must enable {nameof(AuthorizationBearerTokenFactory)}.{nameof(Enable)}");
        VerifyBearerTokenGetterFuncIsSet();
        return _getBearerTokenAsyncFunc!(cancellationToken);
    }

    /// <summary>
    /// Checks if the delegate for fetching bearer tokens is set.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="SetBearerTokenGetterFunc"/> not called prior to calling this method
    /// </exception>
    public static void VerifyBearerTokenGetterFuncIsSet()
    {
        if (!_enabled) return;
        if (_getBearerTokenAsyncFunc is null)
        {
            throw new InvalidOperationException(
                $"Cannot call {nameof(AuthorizationBearerTokenFactory)}.{nameof(GetBearerTokenAsync)} without calling {nameof(AuthorizationBearerTokenFactory)}.{nameof(SetBearerTokenGetterFunc)} first");
        }
    }

    /// <summary>
    /// Enables the use of this class. Turning it on makes calling <see cref="SetBearerTokenGetterFunc"/> mandatory.
    /// Leaving it off makes that optional, but will cause an <see cref="InvalidOperationException"/> to be thrown
    /// from <see cref="GetBearerTokenAsync" /> if that's called without enabling.
    /// </summary>
    public static void Enable() => _enabled = true;

    public static void SetBearerTokenGetterFunc(Func<CancellationToken, Task<string>> getBearerTokenAsyncFunc)
        => _getBearerTokenAsyncFunc = getBearerTokenAsyncFunc;
}
