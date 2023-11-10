using Core7Library;
using Core7Library.CatFacts;
using Microsoft.Extensions.Options;

namespace WorkServiceSeven;

public class Worker7 : BackgroundService
{
    private readonly ILogger<Worker7> _logger;
    private readonly MySettings _settings;
    private readonly ICatFactsClient _catFactsHttpClient;

    public Worker7(ILogger<Worker7> logger, IOptions<MySettings> settings, ICatFactsClient catFactsHttpClient)
    {
        _logger = logger;
        _settings = settings.Value;
        _catFactsHttpClient = catFactsHttpClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start! Current Environment: {CurrentEnvironmentName}", CurrentEnvironment.Name);
        while (!stoppingToken.IsCancellationRequested)
        {
            List<CatFact> theFacts = await _catFactsHttpClient.GetTheFacts();
            Console.WriteLine("Found {0} Cat Facts!", theFacts.Count);
            await Task.Delay(500, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StopAsync!");
        return base.StopAsync(cancellationToken);
    }
}
