using System.Diagnostics;
using Core7Library.Extensions;
using Microsoft.Extensions.Logging;

namespace Core7Library;

public class HttpLoggingHandler : DelegatingHandler
{
    private static readonly Stopwatch Stopwatch = new();
    private readonly ILogger<HttpLoggingHandler> _logger;

    public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Guid id = Guid.NewGuid();
        _logger.LogDebug("[{Id}] Request: {Request}", id, request);
        if (request.ContentIsTextBased())
        {
            var requestContent = await request.Content!.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("[{Id}] Content: {RequestContent}", id, requestContent.TryFormatJson());
        }
        Stopwatch.Restart();
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        Stopwatch.Stop();
        _logger.LogDebug("[{Id}] Response [{RequestDuration}]: {Response}", id, Stopwatch.Elapsed, response);
        if (response.ContentIsTextBased())
        {
            Stopwatch.Restart();
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Stopwatch.Stop();
            _logger.LogDebug("[{Id}] Response Content [{ResponseReadDuration}]: {ResponseContent}", id, Stopwatch.Elapsed, responseContent.TryFormatJson());
        }
        return response;
    }
}
