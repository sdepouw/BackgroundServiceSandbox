using Core7Library;
using WorkServiceSeven;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((context, services) =>
{
    services.AddHostedService<Worker7>();
    services.Configure<MySettings>(context.Configuration.GetSection(nameof(MySettings)));
});

IHost host = builder.Build();
host.Run();
