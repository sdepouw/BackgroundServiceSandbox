using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

public class HttpLoggingHandler : DelegatingHandler
{
    private readonly ILogger<HttpLoggingHandler> _logger;

    public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var req = request;
        var id = Guid.NewGuid().ToString();
        var msg = $"[{id} -   Request]";

        _logger.LogInformation($"{msg}========Start==========");
        _logger.LogInformation($"{msg} {req.Method} {req.RequestUri.PathAndQuery} {req.RequestUri.Scheme}/{req.Version}");
        _logger.LogInformation($"{msg} Host: {req.RequestUri.Scheme}://{req.RequestUri.Host}");

        foreach (var header in req.Headers)
            _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

        if (req.Content != null)
        {
            foreach (var header in req.Content.Headers)
                _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (req.Content is StringContent || IsTextBasedContentType(req.Headers) ||
                this.IsTextBasedContentType(req.Content.Headers))
            {
                var result = await req.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogInformation($"{msg} Content:");
                _logger.LogInformation($"{msg} {result}");
            }
        }

        var start = DateTime.Now;

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        var end = DateTime.Now;

        _logger.LogInformation($"{msg} Duration: {end - start}");
        _logger.LogInformation($"{msg}==========End==========");

        msg = $"[{id} - Response]";
        _logger.LogInformation($"{msg}=========Start=========");

        var resp = response;

        _logger.LogInformation(
            $"{msg} {req.RequestUri.Scheme.ToUpper()}/{resp.Version} {(int) resp.StatusCode} {resp.ReasonPhrase}");

        foreach (var header in resp.Headers)
            _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

        if (resp.Content != null)
        {
            foreach (var header in resp.Content.Headers)
                _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (resp.Content is StringContent || this.IsTextBasedContentType(resp.Headers) ||
                this.IsTextBasedContentType(resp.Content.Headers))
            {
                start = DateTime.Now;
                var result = await resp.Content.ReadAsStringAsync();
                end = DateTime.Now;

                _logger.LogInformation($"{msg} Content:");
                _logger.LogInformation($"{msg} {result}");
                _logger.LogInformation($"{msg} Duration: {end - start}");
            }
        }

        _logger.LogInformation($"{msg}==========End==========");
        return response;
    }

    readonly string[] types = new[] {"html", "text", "xml", "json", "txt", "x-www-form-urlencoded"};


    bool IsTextBasedContentType(HttpHeaders headers)
    {
        IEnumerable<string> values;
        if (!headers.TryGetValues("Content-Type", out values))
            return false;
        var header = string.Join(" ", values).ToLowerInvariant();

        return types.Any(t => header.Contains(t));
    }
}
