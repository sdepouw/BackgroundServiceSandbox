using Core7Library;
using WorkServiceEight;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker8>();
builder.Services.Configure<MySettings>(builder.Configuration.GetSection(nameof(MySettings)));

IHost host = builder.Build();
host.Run();
