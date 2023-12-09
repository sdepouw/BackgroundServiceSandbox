using Core8Library.SuperBuilder;
using Core8WebAPI;
using Microsoft.Extensions.Options;

SuperWebApplicationBuilder builder = SuperWebApplicationBuilder.Create();
builder.WithSettings<ExampleSettings>();

var app = builder.BuildAndValidate();

app.MapGet("/", () => "Try going to /foo/whatever!");
app.MapGet("/foo/{myVal}", DoTheNeedful);

app.Run();

static async Task<IResult> DoTheNeedful(string? myVal, IOptions<ExampleSettings> settings, IExampleHandler dep, CancellationToken token)
{
    if (myVal == "aaa") return TypedResults.BadRequest();
    await dep.GimmeAsync(token);
    return TypedResults.Ok(settings.Value);
}
