using Core8Library.SuperBuilder;
using Core8WebAPI;

SuperWebApplicationBuilder builder = SuperWebApplicationBuilder.Create();
builder.WithSettings<ExampleSettings>();

var app = builder.BuildAndValidate();

app.MapGet("/", () => "Try going to /real and /staff! /foo/whatever, /bar, and /kaboom for legacy.");
app.MapGet("/foo/{myVal}", SomeClass.DoTheNeedful);
app.MapGet("/bar", SomeClass.TestingDI);
app.MapGet("/kaboom", () => { throw new NotImplementedException("Nobody here but us trees!"); });

app.MapGet("/real", (CancellationToken token) => app.Services.GetRequiredService<IExampleHandler>().GimmeAsync(token));
app.MapGet("/stuff", (CancellationToken token) => app.Services.GetRequiredService<IDifferentHandler>().GimmeLongWaitAsync(token));

app.Run();
