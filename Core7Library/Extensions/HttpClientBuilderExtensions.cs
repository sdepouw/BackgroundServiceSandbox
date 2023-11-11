using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Core7Library.Extensions;

public static class HttpClientBuilderExtensions
{
    /// <summary>
    /// If you want redirects to not happen for your Refit client, and you are using
    /// <see cref="RefitSettings.AuthorizationHeaderValueGetter"/>, this method will
    /// try to set <see cref="HttpClientHandler.AllowAutoRedirect"/> to false.
    /// If the primary handler for this client is not an instance of <see cref="DelegatingHandler"/>,
    /// or if its inner handler is not an instance of <see cref="HttpClientHandler"/>,
    /// this method does nothing.
    /// </summary>
    public static IHttpClientBuilder DisableAutoRedirect(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.ConfigureHttpMessageHandlerBuilder(builder =>
        {
            if (builder.PrimaryHandler is not DelegatingHandler delegatingHandler) return;
            if (delegatingHandler.InnerHandler is not HttpClientHandler inner) return;
            inner.AllowAutoRedirect = false;
        });
    }
}
