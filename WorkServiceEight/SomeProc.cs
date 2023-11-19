using Core7Library.CatFacts;

namespace WorkServiceEight;

public class SomeProc(string name, ICatFactsClientService catFactsClientService)
{
    public bool DontStopMeNow { get; private set; }
    private string Name { get; set; } = name;

    public async Task Start(CancellationToken cancellationToken)
    {
        DontStopMeNow = true;
        Console.WriteLine("Starting {0}. Don't stop me now!", Name);
        List<CatFact> theFacts = await catFactsClientService.GetTheFactsAsync(cancellationToken);
        Console.WriteLine("{0} Done! Found {1} Cat Facts!", Name, theFacts.Count);
        await catFactsClientService.Explode(cancellationToken);
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
