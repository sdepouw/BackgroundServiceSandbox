namespace WorkServiceSeven;

public class Worker7 : BackgroundService
{
    private readonly ILogger<Worker7> _logger;

    public Worker7(ILogger<Worker7> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker7 running at with version {version}", Environment.Version);
            await Task.Delay(1000, stoppingToken);
            _logger.LogInformation("Post delay! {time}", DateTimeOffset.Now);
        }
        _logger.LogInformation("Stopped!");
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StopAsync!");
        return base.StopAsync(cancellationToken);
    }
}
