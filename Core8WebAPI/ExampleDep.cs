using Microsoft.Extensions.Options;

namespace Core8WebAPI;

public class ExampleDep(IOptions<ExampleSettings> settings) : IExampleDep
{
    private readonly ExampleSettings _settings = settings.Value;

    public Task<string> GimmeAsync(CancellationToken cancellationToken) => Task.FromResult($"Hello World! My Name? {_settings.Name} | Token! {cancellationToken.IsCancellationRequested}");
}
