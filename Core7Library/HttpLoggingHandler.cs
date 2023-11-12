using System.Diagnostics;
using System.Net.Http.Headers;
using Core7Library.Extensions;
using Microsoft.Extensions.Logging;

namespace Core7Library;

public class HttpLoggingHandler : DelegatingHandler
{
    private readonly ILogger<HttpLoggingHandler> _logger;

    public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Request}", request);
        if (IsTextBasedContentType(request.Content))
        {
            var requestContent = await request.Content!.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("Content: {RequestContent}", requestContent.TryFormatJson());
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        _logger.LogDebug("Request Duration: {RequestDuration}", stopwatch.Elapsed);
        _logger.LogDebug("{Response}", response);
        if (IsTextBasedContentType(response.Content))
        {
            stopwatch.Restart();
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            stopwatch.Stop();
            _logger.LogDebug("Response Read Duration: {ResponseReadDuration}", stopwatch.Elapsed);
            _logger.LogDebug("Response Content: {ResponseContent}", responseContent.TryFormatJson());
        }
        return response;
    }

    private readonly string[] _types = {"html", "text", "xml", "json", "txt", "x-www-form-urlencoded"};
    private bool IsTextBasedContentType(HttpContent? content)
    {
        if (content is null) return false;
        return content is StringContent || IsTextBasedContentType(content.Headers) || IsTextBasedContentType(content.Headers);
    }

    private bool IsTextBasedContentType(HttpHeaders headers)
    {
        if (!headers.TryGetValues("Content-Type", out IEnumerable<string>? values))
            return false;
        var header = string.Join(" ", values).ToLowerInvariant();

        return _types.Any(t => header.Contains(t));
    }
}
