namespace Core7Library.TypedHttpClient;

/// <summary>
/// Used to create instances of typed <see cref="HttpClient"/> classes, so that
/// long-running instances, singletons, etc. don't end up with stale <see cref="HttpClient"/>
/// instances by directly injecting typed <see cref="HttpClient"/>s.
/// </summary>
public interface ITypedHttpClientFactory<TTypedHttpClient>
    where TTypedHttpClient : ITypedHttpClient
{
    TTypedHttpClient CreateClient();

}
