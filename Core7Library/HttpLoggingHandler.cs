using System.Diagnostics;
using Core7Library.Extensions;
using Microsoft.Extensions.Logging;

namespace Core7Library;

/// <summary>
/// Logs and times HTTP requests/responses, including all content (as <see cref="LogLevel.Debug"/>);
/// each HTTP request is logged alongside a unique ID for easier matching/filtering
/// </summary>
/// <remarks>
/// CAUTION: Services with heavy HTTP traffic that log <see cref="LogLevel.Debug" />-level messages can potentially
/// log a lot of information!
/// </remarks>
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
        Stopwatch.Restart();
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        Stopwatch.Stop();
        _logger.LogInformation("[{Id}] Request: {Request}", id, request);
        _logger.LogInformation("[{Id}] Response: {Response}", id, response);
        if (request.ContentIsTextBased())
        {
            var requestContent = await request.Content!.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("[{Id}] Request Content: {RequestContent}", id, requestContent.TryFormatJson());
        }
        if (response.ContentIsTextBased())
        {
            Stopwatch.Restart();
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Stopwatch.Stop();
            _logger.LogInformation("[{Id}] Response Content [{ResponseReadDuration}]: {ResponseContent}", id, Stopwatch.Elapsed, responseContent.TryFormatJson());
        }
        return response;
    }
}
