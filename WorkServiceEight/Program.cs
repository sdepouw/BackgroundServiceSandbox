using Core7Library.BearerTokenStuff;
using Core7Library.CatFacts;
using Core8Library;
using WorkServiceEight;

SuperHostBuilder superBuilder = SuperHostBuilder.Create<Worker8>();
var mySettings = superBuilder.WithSettings<MySettings>();
var catFactsSettings = superBuilder.WithSettings<CatFactsClientSettings>();
IHost host = superBuilder
    .WithRefitClient<IOAuthClient>(c => c.BaseAddress = new Uri("https://example.com/"), enableRequestResponseLogging: mySettings.EnableHttpRequestResponseLogging)
    .WithRefitClient<ICatFactsClient>(ConfigureCatFactsClient, getBearerTokenAsyncFunc: GetBearerTokenAsyncFunc, enableRequestResponseLogging: mySettings.EnableHttpRequestResponseLogging)
    .BuildAndValidate();

host.Run();

void ConfigureCatFactsClient(HttpClient c)
{
    c.BaseAddress = new Uri(catFactsSettings.Host);
    c.Timeout = TimeSpan.FromSeconds(3);
}

Task<string> GetBearerTokenAsyncFunc(IHost createdHost, CancellationToken token)
    => createdHost.Services.GetRequiredService<IOAuthClientService>().GetBearerTokenAsync(token);
