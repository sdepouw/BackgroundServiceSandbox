using Microsoft.Extensions.DependencyInjection;

namespace Core7Library;

public class CatFactsClientFactory : ICatFactsClientFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CatFactsClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICatFactsClient CreateClient() => _serviceProvider.GetRequiredService<ICatFactsClient>();
}