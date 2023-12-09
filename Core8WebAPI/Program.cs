using Core8Library.SuperBuilder;
using Core8WebAPI;

SuperWebApplicationBuilder builder = SuperWebApplicationBuilder.Create();
builder.WithSettings<ExampleSettings>();

var app = builder.BuildAndValidate();

app.MapGet("/", () => "Try going to /foo/whatever! Or /bar if you have nothing better to do for a while!");
app.MapGet("/foo/{myVal}", SomeClass.DoTheNeedful);
app.MapGet("/bar", SomeClass.TestingDI);

app.Run();
