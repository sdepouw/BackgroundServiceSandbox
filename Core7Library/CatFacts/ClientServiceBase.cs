using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Refit;

namespace Core7Library.CatFacts;

/// <summary>
/// Class to inherit for any client service class that wraps access to Refit client interfaces.
/// Provides means for making calls to Refit clients and handling exceptions, non-successful
/// status codes, etc. Assumes requests are wrapped in <see cref="ApiResponse{T}"/>.
/// </summary>
public abstract class ClientServiceBase
{
    /// <summary>
    /// Logger provided by the derived class
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Constructor to provide base class dependencies from the derived class
    /// </summary>
    protected ClientServiceBase(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Makes an HTTP request using the supplied <paramref name="apiCall"/>.
    /// If an <see cref="ApiException"/> is thrown, an empty list <see cref="List{T}"/>
    /// is returned instead and the exception is logged.
    /// </summary>
    /// <param name="apiCall">The API call to make. Usually "() => _client.MyCall()", where "MyCall()" returns
    /// an instance of <see cref="ApiResponse{T}"/></param>
    /// <param name="ensureSuccessStatusCode">Default true. When enabled, throws an <see cref="ApiException"/> if the response status code does not indicate success.</param>
    /// <param name="caller">The name of the calling method, for logging purposes. Auto-filled.</param>
    /// <typeparam name="T">The type of object to return.</typeparam>
    protected Task<List<T>> MakeRequestListAsync<T>(Func<Task<ApiResponse<List<T>?>>> apiCall, bool ensureSuccessStatusCode = true, [CallerMemberName]string caller = "")
        => MakeRequestAsync(apiCall, new List<T>(), ensureSuccessStatusCode, caller)!;

    /// <summary>
    /// Makes an HTTP request using the supplied <paramref name="apiCall"/>, awaiting for the result.
    /// If an <see cref="ApiException"/> is thrown, <paramref name="defaultIfError"/>
    /// (null if not set) is returned instead and the exception is logged.
    /// </summary>
    /// <param name="apiCall">The API call to make. Usually "() => _client.MyCall()", where "MyCall()" returns
    /// an instance of <see cref="ApiResponse{T}"/></param>
    /// <param name="defaultIfError">The value to return if an error occurs. Default to null.</param>
    /// <param name="ensureSuccessStatusCode">Default true. When enabled, throws an <see cref="ApiException"/> if the response status code does not indicate success.</param>
    /// <param name="caller">The name of the calling method, for logging purposes. Auto-filled.</param>
    /// <typeparam name="T">The type of object to return.</typeparam>
    protected async Task<T?> MakeRequestAsync<T>(Func<Task<ApiResponse<T?>>> apiCall,
        T? defaultIfError = null,
        bool ensureSuccessStatusCode = true,
        [CallerMemberName]string caller = "")
        where T : class
    {
        try
        {
            ApiResponse<T?> response = await apiCall();
            if (ensureSuccessStatusCode) await response.EnsureSuccessStatusCodeAsync();
            return response.Content;
        }
        catch (ApiException ex)
        {
            string reasonPhrase = ex.ReasonPhrase ?? "N/A";
            string content = ex.Content ?? "N/A";
            Logger.LogError(ex, "HTTP request {RequestMethodName} failed. Reason: {ReasonPhrase} | Content: {Content}",
                caller, reasonPhrase, content);
            return defaultIfError;
        }
    }
}
