using System.Reflection;
using Core8Library.SuperBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core8Library;

internal static class HostValidator
{
    /// <summary>
    /// Validates each of the settings types given (DataAnnotations, and any database connection strings);
    /// also validates that Serilog does not throw when logging (i.e. has permissions to write to file)
    /// </summary>
    /// <param name="host">The created <see cref="IHost"/> instance</param>
    /// <param name="settingsTypesToValidate">Collection of types that represent settings, to be validated</param>
    /// <exception cref="ApplicationException">Thrown if any validation error occurs</exception>
    public static void Validate(IHost host, List<Type> settingsTypesToValidate)
    {
        // Validates Serilog
        EnableSerilogValidation();
        var logger = host.Services.GetRequiredService<ILogger<SuperHostBuilder>>();
        logger.LogDebug("[{Validator}] Beginning validation", nameof(HostValidator));
        Serilog.Debugging.SelfLog.Disable();

        // Validates all settings
        bool allSettingsValid = settingsTypesToValidate.All(t => ValidateSettings(t, host.Services, logger));
        if (!allSettingsValid)
        {
            throw new HostValidationException("Not all settings were found to be valid");
        }
    }

    private static bool ValidateSettings(Type settingsToValidate, IServiceProvider services, ILogger logger)
    {
        try
        {
            // TODO: Fetch Options<TSettings>.
            // TODO: If a connection string is present, validate that as well.
            // var options = services.GetRequiredService(typeof(IOptions<object>));
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Validator}] {SettingsType} validation error", nameof(HostValidator), settingsToValidate.Name);
            return false;
        }
    }

    private static void EnableSerilogValidation()
    {
        Serilog.Debugging.SelfLog.Enable(message =>
        {
            string appIdentifier = Assembly.GetExecutingAssembly().FullName?.Split(",").FirstOrDefault() ?? "Program";
            var logPath = Path.Join(AppContext.BaseDirectory, $"[{DateTime.UtcNow:yyyy-MM-dd HH.mm.ss}]-[{appIdentifier}]-SerilogError.txt");
            var messageToLog = $"[{nameof(HostValidator)}] Serilog Validation Error - {message}";
            try
            {
                File.AppendAllText(logPath, messageToLog);
                throw new HostValidationException($"[{nameof(HostValidator)}] Serilog Validation Error - Check [{logPath}] for errors");
            }
            catch (Exception ex)
            {
                // Last resort. Most likely means we won't see these errors when running as a Windows service
                Console.WriteLine($"[{nameof(HostValidator)}] Serilog Validation Error: {message}");
                Console.WriteLine($"[{nameof(HostValidator)}] COULD NOT WRITE THIS TO FILE: {ex}");
                throw new HostValidationException($"Serilog Validation Error: {message}{Environment.NewLine}COULD NOT WRITE THIS EXCEPTION TO FILE: {ex}");
            }
        });
    }
}
