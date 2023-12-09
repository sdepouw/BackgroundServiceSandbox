using Microsoft.Extensions.Options;

namespace Core8WebAPI.Filters;

public class APIGuardFilter(IOptions<ExampleSettings> settings) : IEndpointFilter
{
    private readonly ExampleSettings _settings = settings.Value;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var secretHeader = httpContext.Request.Headers["Secret"];
        if (secretHeader != _settings.APISecretKey)
        {
            return Results.NotFound();
        }
        return await next(context);
    }
}
