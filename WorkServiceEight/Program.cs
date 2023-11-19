using Core7Library.BearerTokenStuff;
using Core7Library.CatFacts;
using Core8Library;
using WorkServiceEight;

SuperHostBuilder superBuilder = SuperHostBuilder.Create<Worker8>();
var mySettings = superBuilder.WithSettings<MySettings>();
var catFactsSettings = superBuilder.WithSettings<CatFactsClientSettings>();
superBuilder.WithRefitClient<IOAuthClient>(c => c.BaseAddress = new Uri("https://example.com/"),
    enableRequestResponseLogging: mySettings.EnableHttpRequestResponseLogging);
superBuilder.WithRefitClient<IOAuthClient>(c => c.BaseAddress = new Uri(catFactsSettings.Host),
    getBearerTokenAsyncFunc: GetBearerTokenAsyncFunc, enableRequestResponseLogging: mySettings.EnableHttpRequestResponseLogging);
IHost host = superBuilder.BuildAndValidate();
host.Run();

Task<string> GetBearerTokenAsyncFunc(IHost createdHost, CancellationToken token)
    => createdHost.Services.GetRequiredService<IOAuthClientService>().GetBearerTokenAsync(token);
