using Core8WebAPI;
using Core8WebAPI.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace TestingTime.EndpointFilterTests;

public class RequestResponseLogFilterTests : EndpointFilterTestBase
{
    private readonly ExampleSettings _settings = new();
    private readonly RequestResponseLogFilter _filter;
    private readonly ILogger<RequestResponseLogFilter> _mockedLogger = Substitute.For<ILogger<RequestResponseLogFilter>>();
    protected override IEndpointFilter EndpointFilter => _filter;

    public RequestResponseLogFilterTests() => _filter = new(_mockedLogger, Options.Create(_settings));

    [Fact]
    public async Task LogsNothingWhenRequestResponseLoggingDisabled()
    {
        _settings.LogHttpRequestResponse = false;

        await InvokeEndpointFilterAsync();

        _mockedLogger.DidNotReceiveWithAnyArgs().Log(Arg.Any<LogLevel>(), Arg.Any<EventId>(), Arg.Any<object?>(), Arg.Any<Exception>(), Arg.Any<Func<object?, Exception?, string>>());
    }

    [Fact]
    public async Task LogsSomethingWhenRequestResponseLoggingEnabled()
    {
        _settings.LogHttpRequestResponse = true;

        await InvokeEndpointFilterAsync();

        _mockedLogger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<EventId>(), Arg.Any<object?>(), Arg.Any<Exception>(), Arg.Any<Func<object?, Exception?, string>>());
    }


}