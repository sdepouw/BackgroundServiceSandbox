using Core7Library.CatFacts;
using Microsoft.Extensions.Options;

namespace WorkServiceSeven;

public class Worker7 : BackgroundService
{
    private readonly ILogger<Worker7> _logger;
    private readonly MySettings _settings;
    private readonly ICatFactsService _service;

    public Worker7(ILogger<Worker7> logger, IOptions<MySettings> settings, ICatFactsService service)
    {
        _logger = logger;
        _service = service;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // List<Task> tasks = new();
        // for (int i = 0; i < 10; i++)
        // {
        //     tasks.Add(DoSomethingAsync(stoppingToken, i));
        // }
        //
        // await Task.WhenAll(tasks);
        // _logger.LogInformation("done!");

        //_logger.LogInformation("Start! Current Environment: {CurrentEnvironmentName}", CurrentEnvironment.Name);
        while (!stoppingToken.IsCancellationRequested)
        {
            // List<CatFact> theFacts = await _service.GetTheFactsAsync(stoppingToken);
            // _logger.LogInformation("Found {0} Cat Facts!", theFacts.Count);
            List<Task> tasks = new()
            {
                _service.GetTheFactsAsync(stoppingToken),
                // _service.GetTheFactsAsync(stoppingToken),
                // _service.Hmmm(stoppingToken)
            };
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "OOOPS!");
            }
            _logger.LogInformation("Done doing stuff.");
            await Task.Delay(1000 * 60 * 10, stoppingToken);
        }
    }

    private  Task DoSomethingAsync(CancellationToken token, int index)
    {
        // try
        // {
            if (index == 3)
            {
                throw new Exception("OH NO");
            }

            var ex = Task.Delay(3000, token).Exception;
            _logger.LogInformation("{i} Won!", index);
            return Task.CompletedTask;
            // }
            // catch (Exception e)
            // {
            //     _logger.LogError(e, "Something's wrong! {i}", index);
            //     throw;
            // }
    }


    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StopAsync!");
        return base.StopAsync(cancellationToken);
    }
}
