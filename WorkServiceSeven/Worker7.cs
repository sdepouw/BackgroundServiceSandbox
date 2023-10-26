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
        List<CatFact> theFacts = await _catFactsHttpClient.GetTheFacts();
        Console.WriteLine("Found {0} Cat Facts!", theFacts.Count);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker7 running at with version {version}", Environment.Version);
            _logger.LogInformation("Settings! {Foo} | {Bar} | {Fizz} | {Buzz}",
                _settings.Foo, _settings.Bar, _settings.Fizz, _settings.Buzz);
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StopAsync!");
        return base.StopAsync(cancellationToken);
    }
}
