using System.Runtime.CompilerServices;
using System.Text.Json;
using Core7Library.BearerTokenStuff;
using Core7Library.Extensions;
using Microsoft.Extensions.Logging;
using Refit;

namespace Core7Library.CatFacts;

/// <summary>
/// Class to inherit for any class that accesses Refit client interfaces.
/// Provides means for making calls to Refit clients and handling exceptions, non-successful
/// status codes, etc. Assumes requests are wrapped in <see cref="ApiResponse{T}"/>.
/// </summary>
/// <typeparam name="TRefitClient">The Refit client interface in use.</typeparam>
public abstract class RefitClientServiceBase<TRefitClient>
{
    /// <summary>
    /// Logger provided by the derived class
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// The instance of Refit client
    /// </summary>
    protected readonly TRefitClient RefitClient;

    /// <summary>
    /// Constructor to provide base class dependencies from the derived class
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="AuthorizationBearerTokenFactory.SetBearerTokenGetterFunc"/> not called prior
    /// </exception>
    protected RefitClientServiceBase(TRefitClient refitClient, ILogger logger)
    {
        RefitClient = refitClient;
        Logger = logger;
        CheckClientBearerTokenUsage();
    }

    private void CheckClientBearerTokenUsage()
    {
        var attrs = typeof(TRefitClient).GetCustomAttributes(false).Where(attr => attr is HeadersAttribute).Cast<HeadersAttribute>();
        var usingBearerTokens = attrs.Any(attr => attr.Headers.Any(h => h.StartsWith("Authorization: Bearer", StringComparison.CurrentCultureIgnoreCase)));
        if (usingBearerTokens)
        {
            AuthorizationBearerTokenFactory.VerifyBearerTokenGetterFuncIsSet(Logger);
        }
    }

    /// <summary>
    /// Makes an HTTP request using the supplied <paramref name="apiCall"/>, awaiting the response.
    /// If an <see cref="ApiException"/> is thrown, null is returned
    /// </summary>
    /// <param name="apiCall">The API call to make</param>
    /// <param name="ensureSuccessStatusCode">When enabled, throws an <see cref="ApiException"/> if the response status code does not indicate success (default true)</param>
    /// <param name="caller">The name of the calling method, for logging purposes (auto-filled)</param>
    /// <typeparam name="T">The type of object to return</typeparam>
    protected Task<T?> GetApiResponse<T>(Task<ApiResponse<T?>> apiCall, bool ensureSuccessStatusCode = true, [CallerMemberName] string caller = "")
        where T : class => GetApiResponseInternal(apiCall, null, ensureSuccessStatusCode, caller);

    /// <summary>
    /// Makes an HTTP request using the supplied <paramref name="apiCall"/>, awaiting the response.
    /// If an <see cref="ApiException"/> is thrown, an empty <see cref="List{T}"/> is returned
    /// </summary>
    /// <param name="apiCall">The API call to make</param>
    /// <param name="ensureSuccessStatusCode">When enabled, throws an <see cref="ApiException"/> if the response status code does not indicate success (default true)</param>
    /// <param name="caller">The name of the calling method, for logging purposes (auto-filled)</param>
    /// <typeparam name="T">The type of object to return within the List</typeparam>
    protected Task<List<T>> GetApiResponse<T>(Task<ApiResponse<List<T>?>> apiCall, bool ensureSuccessStatusCode = true, [CallerMemberName]string caller = "")
        => GetApiResponseInternal(apiCall, new List<T>(), ensureSuccessStatusCode, caller)!;

    /// <summary>
    /// Makes an HTTP request using the supplied <paramref name="apiCall"/>, awaiting the response.
    /// If an <see cref="ApiException"/> is thrown, <paramref name="defaultOnError"/> is returned
    /// </summary>
    /// <param name="apiCall">The API call to make</param>
    /// <param name="defaultOnError">The value to return if an error occurs</param>
    /// <param name="ensureSuccessStatusCode">When enabled, throws an <see cref="ApiException"/> if the response status code does not indicate success (default true)</param>
    /// <param name="caller">The name of the calling method, for logging purposes (auto-filled)</param>
    /// <typeparam name="T">The type of object to return</typeparam>
    protected Task<T> GetApiResponse<T>(Task<ApiResponse<T?>> apiCall, T defaultOnError, bool ensureSuccessStatusCode = true, [CallerMemberName]string caller = "")
        where T : class => GetApiResponseInternal(apiCall, defaultOnError, ensureSuccessStatusCode, caller)!;

    private async Task<T?> GetApiResponseInternal<T>(Task<ApiResponse<T?>> apiCall, T? defaultOnError, bool ensureSuccessStatusCode, string caller)
        where T : class
    {
        try
        {
            Logger.LogDebug("[{ClientServiceName}.{RequestMethodName}] HTTP request started", GetType().Name, caller);
            ApiResponse<T?> response = await apiCall;
            if (ensureSuccessStatusCode) await response.EnsureSuccessStatusCodeAsync();
            if (response.Error?.InnerException is JsonException) throw response.Error;
            Logger.LogDebug("[{ClientServiceName}.{RequestMethodName}] HTTP request completed", GetType().Name, caller);
            return response.Content;
        }
        catch (ApiException ex) when (ex.InnerException is JsonException)
        {
            Logger.LogError(ex, "[{ClientServiceName}.{RequestMethodName}] HTTP request succeeded, but a JsonException was thrown on deserialization",
                GetType().Name, caller);
            return defaultOnError;
        }
        catch (ApiException ex)
        {
            string reasonPhrase = ex.ReasonPhrase ?? "N/A";
            string content = ex.Content.TryFormatJson() ?? "N/A";
            Logger.LogError(ex, "[{ClientServiceName}.{RequestMethodName}] HTTP request failed: {ReasonPhrase} | Content: {Content}",
                GetType().Name, caller, reasonPhrase, content);
            return defaultOnError;
        }
    }
}
