using Microsoft.Extensions.Options;

namespace Core8WebAPI;

public class SomeClass
{
    public static async Task<IResult> DoTheNeedful(string? myVal, IOptions<ExampleSettings> settings, IExampleHandler dep, CancellationToken token)
    {
        if (myVal == "ABC") return TypedResults.BadRequest("ABC is forbidden. How dare you!");
        await dep.GimmeAsync(token);
        return TypedResults.Ok(settings.Value);
    }

    public static async Task<IResult> TestingDI(IServiceProvider provider, ILogger<SomeClass> logger, CancellationToken token)
    {
        var res = await provider.GetRequiredService<IExampleHandler>().GimmeLongWaitAsync(token);
        logger.LogInformation("Result: {Result}", res);
        return Results.Ok(res);
    }
}
