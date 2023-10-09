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

```csharp
public class MySettings
{
    public string Foo { get; set; }
    public int Bar { get; set; }
    public bool Fizz { get; set; }
    public DateTimeOffset Buzz { get; set; }
}
```

```json
{
  "MySettings": {
    "Foo": "Some String Value from AppSettings 7",
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
    services.Configure<MySettings>(context.Configuration.GetSection(nameof(MySettings)));
});
```

### .NET 8

```csharp
builder.Services.Configure<MySettings>(builder.Configuration.GetSection(nameof(MySettings)));
```
