using Core7Library;
using WorkServiceEight;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker8>();
builder.Services.AddOptions<MySettings>()
    .BindConfiguration(nameof(MySettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();

IHost host = builder.Build();
host.Run();
