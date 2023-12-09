using Core8Library.SuperBuilder;
using Core8WebAPI;

SuperHostBuilder builder = SuperHostBuilder.CreateWebApp();
builder.WithSettings<ExampleSettings>();

// TODO: Shouldn't have to cast this BuildAndValidate(). It's the only missing part of the grand plan.
var app = (WebApplication)builder.BuildAndValidate();

app.MapGet("/", async (IExampleDep dep, CancellationToken token) => await dep.GimmeAsync(token));

app.Run();
