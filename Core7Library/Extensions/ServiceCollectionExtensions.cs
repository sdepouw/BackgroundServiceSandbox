using Core7Library.CatFacts;
using Core7Library.TypedHttpClient;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Core7Library.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddRefitHttpClientAndFactory<TTypedHttpClient>(this IServiceCollection services, string host)
        where TTypedHttpClient : class, ITypedHttpClient
    {
        services.AddRefitClient<TTypedHttpClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(host));
        services.AddTransient<ITypedHttpClientFactory<ICatFactsClient>, TypedHttpClientFactory>();
    }
}
