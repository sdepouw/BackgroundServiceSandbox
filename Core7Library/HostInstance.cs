using Microsoft.Extensions.Hosting;

namespace Core7Library;

/// <summary>
/// Holds the instance of the <see cref="IHost"/> being run, so that <see cref="IServiceProvider"/> can be accessed
/// anywhere in the service. Necessary for some startup regarding Refit, delegates, and Dependency Injection.
/// </summary>
public static class HostInstance
{
    private static IHost? _host;

    /// <summary>
    /// Call this after building the instance of <see cref="IHost"/>, before calling <see cref="HostingAbstractionsHostExtensions.Run"/>
    /// </summary>
    /// <param name="host"></param>
    public static void SetHost(IHost host) => _host = host;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> from the set <see cref="IHost"/>
    /// </summary>
    /// <returns>The service provider that can be used to resolve dependencies manually</returns>
    /// <exception cref="ApplicationException">Thrown if <see cref="SetHost"/> was never called</exception>
    public static IServiceProvider GetServiceProvider()
    {
        if (_host is null) throw new ApplicationException("Host must be set in HostServiceProvider!");
        return _host.Services;
    }
}
