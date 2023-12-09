using Core8Library.SuperBuilder;
using Core8WebAPI;

SuperWebApplicationBuilder builder = SuperWebApplicationBuilder.Create();
builder.WithSettings<ExampleSettings>();

// TODO: Shouldn't have to cast this BuildAndValidate(). It's the only missing part of the grand plan.
var app = (WebApplication)builder.BuildAndValidate();

app.MapGet("/", async (IExampleHandler dep, CancellationToken token) => await dep.GimmeAsync(token));

app.Run();
