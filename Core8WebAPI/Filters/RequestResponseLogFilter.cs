using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace Core8WebAPI.Filters;

public class RequestResponseLogFilter(ILogger<RequestResponseLogFilter> logger, IOptions<ExampleSettings> settings) : IEndpointFilter
{
    private static readonly Stopwatch Stopwatch = new();

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!settings.Value.LogHttpRequestResponse) return await next(context);
        Guid uniqueRequestId = Guid.NewGuid();

        const string requestLogMessage = """
                                         
                                         =============================================================
                                         HTTP Request {UniqueRequestId}
                                         =============================================================
                                         """;
        logger.LogInformation(requestLogMessage, uniqueRequestId);
        Stopwatch.Restart();
        var result = await next(context);
        Stopwatch.Stop();

        const string responseLogMessage = """
                                          
                                          =============================================================
                                          HTTP Response {UniqueRequestId}
                                          Response Time: {ResponseTime}
                                          Result: {Result}
                                          =============================================================
                                          """;
        logger.LogInformation(responseLogMessage, uniqueRequestId, Stopwatch.Elapsed, context.HttpContext.Response.StatusCode);
        return result;
    }
}
