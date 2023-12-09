using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core8Library.SuperBuilder;

internal class WebApplicationBuilderWrapper : IHostBuilderWrapper
{
    private readonly WebApplicationBuilder _builder = WebApplication.CreateBuilder();
    public IHost Build() => _builder.Build();
    public IServiceCollection Services => _builder.Services;
    public ConfigurationManager Configuration => _builder.Configuration;
    public void RegisterDependencies(AutofacServiceProviderFactory factory, Action<ContainerBuilder> configure)
    {
        _builder.Host.UseServiceProviderFactory(factory).ConfigureContainer(configure);
    }
}
