using System.Reflection;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Hosting;
using AutofacModule = Autofac.Module;

namespace Core8Library;

/// <summary>
/// Registers dependencies inside this class library, as well as those of the Worker
/// </summary>
/// <param name="concreteTypesToIgnore">
/// Types for Autofac to ignore; pass the implementation type, not the interface type
/// </param>
public class SuperAutofacModule(params Type[] concreteTypesToIgnore) : AutofacModule
{
    protected override void Load(ContainerBuilder builder)
    {
        var interfacesToExclude = new[]
        {
            typeof(IHostedService), // Hosted Services (added outside Autofac)
            typeof(IModule) // Autofac Modules
        };
        Assembly thisAssembly = GetType().Assembly;
        Assembly workerAssembly = Assembly.GetEntryAssembly()!; // Confirmed working for EXE and when running as a Service
        builder.RegisterAssemblyTypes(thisAssembly, workerAssembly)
            .Where(t => !concreteTypesToIgnore.Contains(t))
            .Where(t => !t.FullName?.StartsWith("Refit.Implementation") ?? false)
            .Where(t => !t.GetInterfaces().Any(i => interfacesToExclude.Contains(i)))
            .AsImplementedInterfaces(); // Automatic registration of IFoo -> Foo etc.

        // Singleton registration + registering .NET 8's TimeProvider
        builder
            .RegisterInstance(TimeProvider.System)
            .As<TimeProvider>()
            .SingleInstance();

        // Uncomment and debug to view all the things Autofac will do
        // Application will not start if left uncommented
        // var registeredComponents = builder.Build().ComponentRegistry.Registrations;
    }
}
