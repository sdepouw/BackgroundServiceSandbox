using Core7Library.CatFacts;

namespace WorkServiceSeven;

public class Worker7 : BackgroundService
{
    private readonly ILogger<Worker7> _logger;
    private readonly ICatFactsClientService _clientService;

    public Worker7(ILogger<Worker7> logger, ICatFactsClientService clientService)
    {
        _logger = logger;
        _clientService = clientService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(500, stoppingToken); // So default log messages have time to write.

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                List<CatFact> res = await _clientService.GetTheFactsAsync(stoppingToken);
                _logger.LogInformation("Got {Facts} facts!", res.Count);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "OOOPS!");
            }
            _logger.LogInformation("Done doing stuff.");
            await Task.Delay(1000 * 60 * 10, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StopAsync!");
        return base.StopAsync(cancellationToken);
    }
}
