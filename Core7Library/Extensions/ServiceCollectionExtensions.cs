using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Core7Library.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up <see cref="IOptions{TOptions}"/> for <typeparam name="TSettings" /> and validates any annotations.
    /// </summary>
    public static OptionsBuilder<TSettings> AddRequiredSettings<TSettings>(this IServiceCollection services)
        where TSettings : class
    {
        return services.AddOptions<TSettings>()
            .BindConfiguration(typeof(TSettings).Name)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
