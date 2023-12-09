using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Core8Library.SuperBuilder;

public class SuperWebApplicationBuilder : SuperHostBuilderBase<SuperWebApplicationBuilder, WebApplication>
{
    private readonly WebApplicationBuilder _builder = WebApplication.CreateBuilder();
    protected override IHostApplicationBuilder Builder => _builder;
    protected override WebApplication Build() => _builder.Build();
    protected override void RegisterAutofac(AutofacServiceProviderFactory factory, Action<ContainerBuilder> configure)
        => _builder.Host.UseServiceProviderFactory(factory).ConfigureContainer(configure);

    private SuperWebApplicationBuilder() { }

    /// <summary>
    /// Creates a new <see cref="SuperWebApplicationBuilder"/>, with Serilog logging and dependencies registered
    /// with Autofac
    /// </summary>
    /// <param name="autofacModulesToRegister">Any custom Autofac modules that should also be registered</param>
    public static SuperWebApplicationBuilder Create(params Module[] autofacModulesToRegister)
        => CreateManually()
            .WithLogging()
            .WithDependenciesRegistered(autofacModulesToRegister);

    /// <summary>
    /// Only call this if you want to configure logging and DI manually.
    /// Normally, always call <see cref="Create"/>.
    /// </summary>
    public static SuperWebApplicationBuilder CreateManually() => new();
}
