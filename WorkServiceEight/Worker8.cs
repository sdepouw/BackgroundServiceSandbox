using Core7Library;
using Microsoft.Extensions.Options;

namespace WorkServiceEight;

public class Worker8(ILogger<Worker8> logger, IOptions<MySettings> settings) : BackgroundService
{
    private readonly ILogger<Worker8> _logger = logger;
    private readonly MySettings _settings = settings.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SomeProc proc1 = new("1");
        SomeProc proc2 = new("2");
        SomeProc proc3 = new("3");

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

public class SomeProc(string name)
{
    public bool DontStopMeNow { get; private set; }
    private string Name { get; set; } = name;

    public async Task Start()
    {
        DontStopMeNow = true;
        Console.WriteLine("Starting {0}. Don't stop me now!", Name);
        for (int delay = 0; delay < 10; delay++)
        {
            Console.WriteLine("{0} working...", Name);
            await Task.Delay(1000);
        }
        Console.WriteLine("{0} Done!", Name);
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
