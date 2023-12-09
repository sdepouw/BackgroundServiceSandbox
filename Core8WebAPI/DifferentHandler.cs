using Microsoft.Extensions.Options;

namespace Core8WebAPI;

public class DifferentHandler(ILogger<DifferentHandler> logger, IOptions<ExampleSettings> settings) : IDifferentHandler
{
    private readonly ExampleSettings _settings = settings.Value;

    public async Task<string> GimmeLongWaitAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Waiting for {NumberOfSeconds} second(s)", _settings.HowLongToWaitInSeconds);
            await Task.Delay(1000 * _settings.HowLongToWaitInSeconds, cancellationToken);
            return "Wow, you're patient!";
        }
        catch (TaskCanceledException)
        {
            logger.LogWarning("Cancelled!");
            return $"You got impatient! Look at this cancel: {cancellationToken.IsCancellationRequested}";
        }
        finally
        {
            logger.LogInformation("Whoa. Cancelled? {Cancelled}", cancellationToken.IsCancellationRequested);
        }
    }
}
