using WorkServiceEight;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker8>();

IHost host = builder.Build();
host.Run();
