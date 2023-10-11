using Core7Library;
using WorkServiceSeven;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((context, services) =>
{
    services.AddHostedService<Worker7>();
    services.AddOptions<MySettings>()
        .BindConfiguration(nameof(MySettings))
        .ValidateDataAnnotations()
        .ValidateOnStart();
});

IHost host = builder.Build();
host.Run();
