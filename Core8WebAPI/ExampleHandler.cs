using Microsoft.Extensions.Options;

namespace Core8WebAPI;

public class ExampleHandler(IOptions<ExampleSettings> settings) : IExampleHandler
{
    private readonly ExampleSettings _settings = settings.Value;

    public Task<string> GimmeAsync(CancellationToken cancellationToken) => Task.FromResult($"Hello World! My Name? {_settings.Name} | Token! {cancellationToken.IsCancellationRequested}");
}
