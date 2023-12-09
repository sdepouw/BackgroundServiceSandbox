using Microsoft.Extensions.Options;

namespace Core8WebAPI;

public class ExampleHandler(IOptions<ExampleSettings> settings, ILogger<ExampleHandler> logger) : IExampleHandler
{
    private readonly ExampleSettings _settings = settings.Value;

    public Task<string> GimmeAsync(CancellationToken cancellationToken) => Task.FromResult($"Hello World! My Name? {_settings.Name} | Token! {cancellationToken.IsCancellationRequested}");
    public async Task<string> GimmeLongWaitAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(1000 * 60 * 60, cancellationToken);
            return "Wow, you're patient!";
        }
        catch (TaskCanceledException)
        {
            return $"You got impatient! Look at this cancel: {cancellationToken.IsCancellationRequested}";
        }
        finally
        {
            logger.LogInformation("Whoa. Canceled? {Canceled}", cancellationToken.IsCancellationRequested);
        }
    }
}
