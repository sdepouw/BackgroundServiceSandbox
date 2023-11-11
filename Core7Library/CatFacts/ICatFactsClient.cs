using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace Core7Library.CatFacts;

public interface ICatFactsClient
{
    [Get("/{theRoute}")]
    Task<List<CatFact>> GetTheFactsAsync(string theRoute, CancellationToken cancellationToken);
}

public interface ICatFactsService
{
    Task<List<CatFact>> GetTheFactsAsync(CancellationToken cancellationToken);
}

public class CatFactsService : ServiceClientBase, ICatFactsService
{
    private readonly CatFactsClientSettings _settings;
    private readonly ICatFactsClient _catFactsClient;

    public CatFactsService(IOptions<CatFactsClientSettings> settings, ILogger<CatFactsService> logger, ICatFactsClient catFactsClient)
        : base (logger)
    {
        _settings = settings.Value;
        _catFactsClient = catFactsClient;
    }

    public Task<List<CatFact>> GetTheFactsAsync(CancellationToken cancellationToken)
    {
        Logger.LogDebug("In the service");
        Func<Task> taskWithNoReturn = () =>
        {
            _catFactsClient.GetTheFactsAsync("FoobarThisWillNotWork", cancellationToken);
            return Task.CompletedTask;
        };
        Func<Task<string>> taskWithSimpleReturn = () => Task.FromResult("");
        Func<Task<List<CatFact>>> taskWithListReturn = () => _catFactsClient.GetTheFactsAsync(_settings.GetTheFactsRoute, cancellationToken);
        MakeRequest(taskWithNoReturn);
        MakeRequest(taskWithSimpleReturn);
        return MakeRequestList(taskWithListReturn);
    }
}

public abstract class ServiceClientBase
{
    protected readonly ILogger Logger;

    protected ServiceClientBase(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Makes an HTTP request using the supplied <paramref name="apiCall"/>.
    /// If an <see cref="ApiException"/> is thrown, an empty list <see cref="List{T}"/>
    /// is returned instead and the exception is logged.
    /// </summary>
    /// <param name="apiCall">The API call to make. Usually "() => _client.MyCall()"</param>
    /// <param name="caller">The name of the calling method, for logging purposes. Auto-filled.</param>
    /// <typeparam name="T">The type of object to return.</typeparam>
    protected Task<List<T>> MakeRequestList<T>(Func<Task<List<T>>> apiCall, [CallerMemberName]string caller = "") => MakeRequest(apiCall, new List<T>(), caller)!;

    /// <summary>
    /// Makes an HTTP request using the supplied <paramref name="apiCall"/>.
    /// If an <see cref="ApiException"/> is thrown, it is caught and logged logged.
    /// </summary>
    /// <param name="apiCall">The API call to make. Usually "() => _client.MyCall()"</param>
    /// <param name="caller">The name of the calling method, for logging purposes. Auto-filled.</param>
    protected Task MakeRequest(Func<Task> apiCall, [CallerMemberName] string caller = "")
    {
        return MakeRequest(WrappedTask, caller: caller);

        Task<string> WrappedTask()
        {
            apiCall();
            return Task.FromResult("");
        }
    }

    /// <summary>
    /// Makes an HTTP request using the supplied <paramref name="apiCall"/>.
    /// If an <see cref="ApiException"/> is thrown, <paramref name="defaultIfError"/>
    /// (null if not set) is returned instead and the exception is logged.
    /// </summary>
    /// <param name="apiCall">The API call to make. Usually "() => _client.MyCall()"</param>
    /// <param name="defaultIfError">The value to return if an error occurs. Default to null.</param>
    /// <param name="caller">The name of the calling method, for logging purposes. Auto-filled.</param>
    /// <typeparam name="T">The type of object to return.</typeparam>
    protected Task<T?> MakeRequest<T>(Func<Task<T>> apiCall, T? defaultIfError = null, [CallerMemberName]string caller = "")
        where T : class
    {
        try
        {
            return apiCall()!;
        }
        catch (ApiException ex)
        {
            string reasonPhrase = ex.ReasonPhrase ?? "N/A";
            string content = ex.Content ?? "N/A";
            Logger.LogError(ex, "HTTP request {RequestMethodName} failed. Reason: {ReasonPhrase} | Content: {Content}",
                caller, reasonPhrase, content);
            return Task.FromResult(defaultIfError);
        }
    }
}
