﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core8Library.SuperBuilder;

public class SuperHostApplicationBuilder : SuperHostBuilderBase<SuperHostApplicationBuilder>
{
    private readonly HostApplicationBuilder _builder = Host.CreateApplicationBuilder();
    protected override IHostApplicationBuilder Builder => _builder;
    protected override IHost BuildApp() => Build();
    protected override void RegisterDependencies(AutofacServiceProviderFactory factory, Action<ContainerBuilder> configure)
        => _builder.ConfigureContainer(factory, configure);

    /// <summary>
    /// Creates a new <see cref="SuperHostBuilderBase{TBuilder}"/> configured to run as a Windows service,
    /// with Serilog logging and dependencies registered with Autofac
    /// </summary>
    /// <param name="autofacModulesToRegister">Any custom Autofac modules that should also be registered</param>
    /// <typeparam name="THostedService">The worker service class to register</typeparam>
    public static SuperHostApplicationBuilder Create<THostedService>(params Module[] autofacModulesToRegister)
        where THostedService : class, IHostedService
        => CreateManually<THostedService>()
            .WithLogging()
            .WithDependenciesRegistered(autofacModulesToRegister);

    /// <summary>
    /// Only call this if you want to configure logging and DI manually.
    /// Still registers the app as a hosted service.
    /// Normally, always call <see cref="Create{THostedService}"/>.
    /// </summary>
    public static SuperHostApplicationBuilder CreateManually<THostedService>()
        where THostedService : class, IHostedService
        => new SuperHostApplicationBuilder().WithHostedWindowsService<THostedService>();

    public IHost Build() => _builder.Build();

    /// <summary>
    /// Adds the specified serviced as a Hosted Service and Windows Service
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when building a web application instead of a hosted one</exception>
    private SuperHostApplicationBuilder WithHostedWindowsService<THostedService>()
        where THostedService : class, IHostedService
    {
        Builder.Services.AddHostedService<THostedService>();
        Builder.Services.AddWindowsService();
        return this;
    }
}
