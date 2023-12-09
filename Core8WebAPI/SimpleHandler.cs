using Microsoft.Extensions.Options;

namespace Core8WebAPI;

public class SimpleHandler(IOptions<ExampleSettings> settings) : ISimpleHandler
{
    private readonly ExampleSettings _settings = settings.Value;

    public Task<string> HandleAsync(CancellationToken cancellationToken) => Task.FromResult($"Hello World! My Name? {_settings.Name} | Token! {cancellationToken.IsCancellationRequested}");
}
