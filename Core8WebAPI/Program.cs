using Core8Library.SuperBuilder;
using Core8WebAPI;

SuperWebApplicationBuilder builder = SuperWebApplicationBuilder.Create();
builder.WithSettings<ExampleSettings>();

var app = builder.BuildAndValidate();

app.MapGet("/", () => "Try going to /real and /staff! /foo/whatever, /bar, and /kaboom for legacy.");
app.MapGet("/foo/{myVal}", UsingResultsDemo.DoTheNeedful);
app.MapGet("/bar", UsingResultsDemo.TestingDI);
app.MapGet("/kaboom", () => { throw new NotImplementedException("Nobody here but us trees!"); });

app.MapGet("/real", (CancellationToken token) => app.Services.GetRequiredService<ISimpleHandler>().HandleAsync(token));
app.MapGet("/stuff", (CancellationToken token) => app.Services.GetRequiredService<IPatienceHandler>().HandleAsync(token));

app.Run();
