using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace TestingTime.EndpointFilterTests;

public abstract class EndpointFilterTestBase
{
    /// <summary>
    /// The <see cref="IEndpointFilter"/> under test
    /// </summary>
    protected abstract IEndpointFilter EndpointFilter { get; }
    private readonly object? _nextReturnValue = Guid.NewGuid().ToString();
    private readonly HttpContext _httpContext = new DefaultHttpContext();

    /// <summary>
    /// Appends a given key/value pair as a header to the <see cref="HttpContext"/> that will
    /// get passed to the <see cref="IEndpointFilter"/> under test
    /// </summary>
    protected void AppendRequestHeader(string key, StringValues value)
    {
        _httpContext.Request.Headers.Append(key, value);
    }

    /// <summary>
    /// Calls <see cref="IEndpointFilter.InvokeAsync"/>
    /// </summary>
    protected ValueTask<object?> InvokeEndpointFilterAsync()
    {
        DefaultEndpointFilterInvocationContext endpointFilterContext = new(_httpContext);
        ValueTask<object?> Next(EndpointFilterInvocationContext _) => ValueTask.FromResult(_nextReturnValue);
        return EndpointFilter.InvokeAsync(endpointFilterContext, Next);
    }

    /// <summary>
    /// Returns true when the <see cref="IEndpointFilter"/> under test called "await next(context)"
    /// instead of returning a different result (e.g. <see cref="Results.NotFound"/>);
    /// useful for testing whether or not the <see cref="IEndpointFilter"/> returned a happy path scenario
    /// </summary>
    protected bool EndpointFilterContinued(object? returnedResult) => returnedResult == _nextReturnValue;
}
