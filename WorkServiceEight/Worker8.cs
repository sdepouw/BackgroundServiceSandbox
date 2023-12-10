using Core7Library.CatFacts;

namespace WorkServiceEight;

public class Worker8(ILogger<Worker8> logger, ICatFactsClientService catFactsClientService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(100, stoppingToken);
        var facts = await catFactsClientService.GetTheFactsAsync(stoppingToken);
        logger.LogInformation("{C} FACTS WOW", facts.Count);
        // logger.LogInformation("Start! Current Environment: {CurrentEnvironmentName}", CurrentEnvironment.Name);
        //SomeProc proc1 = new("1", catFactsClientService);
        // SomeProc proc2 = new("2", catFactsClient);
        // SomeProc proc3 = new("3", catFactsClient);

        //stoppingToken.Register(() => proc1.Stop());
        // stoppingToken.Register(() => proc2.Stop());
        // stoppingToken.Register(() => proc3.Stop());
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     //List<Task> tasks = new();
        //     //if (!proc1.DontStopMeNow) tasks.Add(proc1.Start(stoppingToken));
        //     // if (!proc2.DontStopMeNow) tasks.Add(proc2.Start());
        //     // if (!proc3.DontStopMeNow) tasks.Add(proc3.Start());
        //
        //     await Task.WhenAll(tasks);
        //     await Task.Delay(1000 * 60 * 100, stoppingToken);
        // }
    }
}
