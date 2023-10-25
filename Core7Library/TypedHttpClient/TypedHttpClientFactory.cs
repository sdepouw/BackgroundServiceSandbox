using Core7Library.CatFacts;
using Microsoft.Extensions.DependencyInjection;

namespace Core7Library.TypedHttpClient;

public class TypedHttpClientFactory : ITypedHttpClientFactory<ICatFactsClient>
{
    private readonly IServiceProvider _serviceProvider;

    public TypedHttpClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICatFactsClient CreateClient() => _serviceProvider.GetRequiredService<ICatFactsClient>();
}
