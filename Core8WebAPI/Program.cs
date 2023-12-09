using Core7Library;
using Core7Library.CatFacts;
using Core8Library.SuperBuilder;
using Core8WebAPI;
using Core8WebAPI.Filters;
using Core8WebAPI.Handlers;

SuperWebApplicationBuilder builder = SuperWebApplicationBuilder.Create(new Core7LibraryAutofacModule());
builder.WithSettings<ExampleSettings>();
var catSettings = builder.WithSettings<CatFactsClientSettings>();
builder.WithRefitClient<ICatFactsClient>(ConfigureCatFactsClient, enableRequestResponseLogging: true);

void ConfigureCatFactsClient(HttpClient c)
{
    c.BaseAddress = new Uri(catSettings.Host);
    c.Timeout = TimeSpan.FromSeconds(3);
}

var app = builder.BuildAndValidate();

app.MapGet("/", () => "Try going to /real and /staff and /cats! /foo/whatever, /bar, and /kaboom for legacy.");
app.MapGet("/foo/{myVal}", UsingResultsDemo.DoTheNeedful);
app.MapGet("/bar", UsingResultsDemo.TestingDI);
app.MapGet("/kaboom", () => { throw new NotImplementedException("Nobody here but us trees!"); });

app.MapGet("/real", (CancellationToken token) => app.Services.GetRequiredService<ISimpleHandler>().HandleAsync(token));
app.MapGet("/stuff", (CancellationToken token) => app.Services.GetRequiredService<IPatienceHandler>().HandleAsync(token));
app.MapGet("/cats", (CancellationToken token) => app.Services.GetRequiredService<ICatHandler>().HandleAsync(token));

var apiGroup = app.MapGroup("/api").AddEndpointFilter<APIGuardFilter>();
apiGroup.MapGet("/guarded", (CancellationToken token) => app.Services.GetRequiredService<ISecretHandler>().HandleAsync(token));

app.Run();
