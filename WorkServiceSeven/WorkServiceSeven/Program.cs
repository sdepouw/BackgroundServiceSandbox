using WorkServiceSeven;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices(services => { services.AddHostedService<Worker7>(); });

IHost host = builder.Build();
host.Run();
