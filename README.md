# BackgroundServiceSandbox

Messing around with .NET7 and .NET8 BackgroundServices. Seeing what it takes to upgrade a .NET 7 Worker Service to a .NET 8 Worker Service

## Adding Hosted Service

### .NET 7

```csharp
using WorkServiceSeven;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((context, services) =>
{
    services.AddHostedService<Worker7>();
});

IHost host = builder.Build();
host.Run();
```

### .NET 8

```csharp
using WorkServiceEight;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker8>();

IHost host = builder.Build();
host.Run();
```

## Adding Configuration for `IOptions<>`

### Example Settings Class and Config

- Relies on [Microsoft.Extensions.Options.DataAnnotations](https://www.nuget.org/packages/Microsoft.Extensions.Options.DataAnnotations) NuGet package to make `ValidateDataAnnotations()` available.

```csharp
using System.ComponentModel.DataAnnotations;

public class MySettings
{
    [Required]
    public string Foo { get; set; }
    public int Bar { get; set; }
    public bool Fizz { get; set; }
    public DateTimeOffset Buzz { get; set; }
}
```

```json
{
  "MySettings": {
    "Foo": "Some Setting",
    "Bar": 38,
    "Fizz": true,
    "Buzz": "2023-10-09T23:27:52.7454651+00:00"
  }
}
```

### .NET 7

```csharp
builder.ConfigureServices((context, services) =>
{
    services.AddOptions<MySettings>()
        .BindConfiguration(nameof(MySettings))
        .ValidateDataAnnotations()
        .ValidateOnStart();
});
```

### .NET 8

```csharp
builder.Services.AddOptions<MySettings>()
    .BindConfiguration(nameof(MySettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

#### Validation Error Example

```
Unhandled exception. Microsoft.Extensions.Options.OptionsValidationException: DataAnnotation validation failed for 'MySettings' members: 'Foo' with the error: 'The Foo field is required.'.
   at Microsoft.Extensions.Options.OptionsFactory`1.Create(String name)                                                                                                                     
   at Microsoft.Extensions.Options.UnnamedOptionsManager`1.get_Value()                                                                                                                      
   at WorkServiceSeven.Worker7..ctor(ILogger`1 logger, IOptions`1 settings) in D:\Dev\BackgroundServiceSandbox\WorkServiceSeven\Worker7.cs:line 14                                  
   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)                                                                        
   at System.Reflection.ConstructorInvoker.Invoke(Object obj, IntPtr* args, BindingFlags invokeAttr)                                                                                        
   at System.Reflection.RuntimeConstructorInfo.Invoke(BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)                                                     
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteRuntimeResolver.VisitConstructor(ConstructorCallSite constructorCallSite, RuntimeResolverContext context)              
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteVisitor`2.VisitCallSiteMain(ServiceCallSite callSite, TArgument argument)                                              
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteRuntimeResolver.VisitRootCache(ServiceCallSite callSite, RuntimeResolverContext context)                               
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteVisitor`2.VisitCallSite(ServiceCallSite callSite, TArgument argument)                                                  
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteRuntimeResolver.VisitIEnumerable(IEnumerableCallSite enumerableCallSite, RuntimeResolverContext context)               
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteVisitor`2.VisitCallSiteMain(ServiceCallSite callSite, TArgument argument)                                              
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteRuntimeResolver.VisitRootCache(ServiceCallSite callSite, RuntimeResolverContext context)                               
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteVisitor`2.VisitCallSite(ServiceCallSite callSite, TArgument argument)                                                  
   at Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteRuntimeResolver.Resolve(ServiceCallSite callSite, ServiceProviderEngineScope scope)                                    
   at Microsoft.Extensions.DependencyInjection.ServiceProvider.CreateServiceAccessor(Type serviceType)                                                                                      
   at System.Collections.Concurrent.ConcurrentDictionary`2.GetOrAdd(TKey key, Func`2 valueFactory)
   at Microsoft.Extensions.DependencyInjection.ServiceProvider.GetService(Type serviceType, ServiceProviderEngineScope serviceProviderEngineScope)
   at Microsoft.Extensions.DependencyInjection.ServiceProvider.GetService(Type serviceType)
   at Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService(IServiceProvider provider, Type serviceType)
   at Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService[T](IServiceProvider provider)
   at Microsoft.Extensions.Hosting.Internal.Host.StartAsync(CancellationToken cancellationToken)
   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.RunAsync(IHost host, CancellationToken token)
   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.RunAsync(IHost host, CancellationToken token)
   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.Run(IHost host)
   at Program.<Main>$(String[] args) in D:\Dev\BackgroundServiceSandbox\WorkServiceSeven\Program.cs:line 16
```

## Using Typed HttpClient and Refit, Without Lasting Too Long

[Refit](https://github.com/reactiveui/refit) is a handy way to quickly get some API access going, and I wanted to play with it here. It's worked out well, relying on typed HttpClient injection to get the job done. For background services, i.e. long-running, practically-singleton instances, however, typed instances of HttpClient (which amounts to long-lasting HttpClient instances) is not recommended, as HttpClient instances are meant to be used in short bursts. Compare the lifetime of a long-running background service to, say, an MVC API request. The latter is much more amenable to typed clients than the former.

As such, I added a line to limit the lifetime of the handler to 10 minutes. This lets us still inject `ICatFactsClient` directly as we have before, but won't run into potential issues with having a too-long-running `HttpClient` instance.

### .NET 7
```csharp
services.AddRefitClient<ICatFactsClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(catFactsClientSettings.Host))
    .SetHandlerLifetime(TimeSpan.FromMinutes(10));
```
    
### .NET 8
```csharp
builder.Services.AddRefitClient<ICatFactsClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(catFactsClientSettings.Host))
    .SetHandlerLifetime(TimeSpan.FromMinutes(10));
```
