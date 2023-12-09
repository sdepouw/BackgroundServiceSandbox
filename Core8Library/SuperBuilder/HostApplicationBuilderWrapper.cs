using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core8Library.SuperBuilder;

internal class HostApplicationBuilderWrapper : IHostBuilderWrapper
{
    private readonly HostApplicationBuilder _builder = Host.CreateApplicationBuilder();

    public IHost Build() => _builder.Build();
    public IServiceCollection Services => _builder.Services;
    public ConfigurationManager Configuration => _builder.Configuration;

    public void RegisterDependencies(AutofacServiceProviderFactory factory, Action<ContainerBuilder> configure)
    {
        _builder.ConfigureContainer(factory, configure);
    }
}
