using Microsoft.Extensions.Options;

namespace Core8WebAPI;

public class UsingResultsDemo
{
    public static async Task<IResult> DoTheNeedful(string? myVal, IOptions<ExampleSettings> settings, ISimpleHandler dep, CancellationToken token)
    {
        if (myVal == "ABC") return TypedResults.BadRequest("ABC is forbidden. How dare you!");
        await dep.HandleAsync(token);
        return TypedResults.Ok(settings.Value);
    }

    public static async Task<IResult> TestingDI(IServiceProvider provider, ILogger<UsingResultsDemo> logger, CancellationToken token)
    {
        var res = await provider.GetRequiredService<IPatienceHandler>().HandleAsync(token);
        logger.LogInformation("Result: {Result}", res);
        return Results.Ok(res);
    }
}
