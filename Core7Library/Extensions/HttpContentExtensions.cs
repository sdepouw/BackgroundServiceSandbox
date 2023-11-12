using System.Net.Http.Headers;

namespace Core7Library.Extensions;

public static class HttpContentExtensions
{
    private static readonly string[] TextBasedTypes = {"html", "text", "xml", "json", "txt", "x-www-form-urlencoded"};

    public static bool ContentIsTextBased(this HttpRequestMessage request) => request.Content.ContentIsTextBased();
    public static bool ContentIsTextBased(this HttpResponseMessage response) => response.Content.ContentIsTextBased();

    private static bool ContentIsTextBased(this HttpContent? content)
    {
        if (content is null) return false;
        return content is StringContent || ContentIsTextBased(content.Headers) || ContentIsTextBased(content.Headers);
    }

    private static bool ContentIsTextBased(this HttpHeaders headers)
    {
        if (!headers.TryGetValues("Content-Type", out IEnumerable<string>? values))
        {
            return false;
        }
        var header = string.Join(" ", values);
        return TextBasedTypes.Any(t => header.Contains(t, StringComparison.InvariantCultureIgnoreCase));
    }
}
