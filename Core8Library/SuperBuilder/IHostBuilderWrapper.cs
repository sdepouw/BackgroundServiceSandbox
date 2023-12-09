using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core8Library.SuperBuilder;

/// <summary>
/// Due to slight differences in how the different builder classes work,
/// wrap under a common interface
/// </summary>
internal interface IHostBuilderWrapper
{
    IHost Build();
    IServiceCollection Services { get; }
    ConfigurationManager Configuration { get; }
    void RegisterDependencies(AutofacServiceProviderFactory factory, Action<ContainerBuilder> configure);
}
