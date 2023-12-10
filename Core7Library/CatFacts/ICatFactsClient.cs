using System.Text.Json.Serialization;
using Core7Library.BearerTokenStuff;
using Refit;

namespace Core7Library.CatFacts;

public interface ICatFactsClient
{
    [Post("/oauth")]
    public Task<ApiResponse<AuthToken?>> GetBearerTokenAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Refit notes:
    /// - Always return <see cref="ApiResponse{T}"/> so that status code can be checked.
    /// - Return type should always be marked as nullable (failed requests return null)
    /// - Can make route Settings-injection capable by passing a string in for the whole route, or part of it
    /// - Last parameter can be cancellation token and it'll work as expected
    /// </summary>
    /// <param name="theRoute">The route to call. Injected instead of hardcoded so that it can be configured</param>
    /// <param name="cancellationToken">CancellationToken to respect.</param>
    /// <returns>Just the facts.</returns>
    [Get("/{theRoute}")]
    [Headers("Authorization: Bearer")]
    Task<ApiResponse<List<CatFact>?>> GetTheFactsAsync(string theRoute, CancellationToken cancellationToken);

    /// <summary>
    /// This is not a real route. It'll just 404.
    /// </summary>
    /// <returns>Nothing!</returns>
    [Post("/this-is-not-real/{theRoute}")]
    Task<ApiResponse<CatFact?>> GetSingleFact(string theRoute, SomeFakeThing theRequest, CancellationToken cancellationToken);
}

public class SomeFakeThing
{
    public string Foo { get; set; } = "Bob";
    public int Bar { get; set; } = 1;
    [JsonPropertyName("TANSTAAFL")]
    public List<OtherThing> Things { get; set; } = new() { new(), new(), new() };
}

public class OtherThing
{
    public string Doof { get; set; } = "Doofing";
    public string Food { get; set; } = "Fooding";
}
