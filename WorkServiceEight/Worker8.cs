using Core7Library;
using Core7Library.CatFacts;
using Core7Library.TypedHttpClient;
using Microsoft.Extensions.Options;

namespace WorkServiceEight;

public class Worker8(ILogger<Worker8> logger, IOptions<MySettings> settings, ITypedHttpClientFactory<ICatFactsClient> httpClientFactory) : BackgroundService
{
    private readonly ILogger<Worker8> _logger = logger;
    private readonly MySettings _settings = settings.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SomeProc proc1 = new("1", httpClientFactory);
        SomeProc proc2 = new("2", httpClientFactory);
        SomeProc proc3 = new("3", httpClientFactory);

        stoppingToken.Register(() => proc1.Stop());
        stoppingToken.Register(() => proc2.Stop());
        stoppingToken.Register(() => proc3.Stop());
        while (!stoppingToken.IsCancellationRequested)
        {
            List<Task> tasks = new();
            if (!proc1.DontStopMeNow) tasks.Add(proc1.Start());
            if (!proc2.DontStopMeNow) tasks.Add(proc2.Start());
            if (!proc3.DontStopMeNow) tasks.Add(proc3.Start());

            await Task.WhenAll(tasks);
            await Task.Delay(1000, stoppingToken);
        }
    }
}

public class SomeProc(string name, ITypedHttpClientFactory<ICatFactsClient> catFactsHttpClientFactory)
{
    public bool DontStopMeNow { get; private set; }
    private string Name { get; set; } = name;

    public async Task Start()
    {
        var catFactsClient = catFactsHttpClientFactory.CreateClient();
        DontStopMeNow = true;
        Console.WriteLine("Starting {0}. Don't stop me now!", Name);
        for (int delay = 0; delay < 10; delay++)
        {
            Console.WriteLine("{0} working...", Name);
            await Task.Delay(1000);
        }

        List<CatFact> theFacts = await catFactsClient.GetTheFacts();
        Console.WriteLine("{0} Done! Found {1} Cat Facts!", Name, theFacts.Count);
        DontStopMeNow = false;
    }

    public async void Stop()
    {
        int delayCounter = 10;
        while (DontStopMeNow && delayCounter > 0)
        {
            Console.WriteLine("Giving {0} a little more time... {1}", Name, delayCounter--);
            await Task.Delay(1000);
        }
        Console.WriteLine("Stopping {0}!", Name);
    }
}
