using Core8WebAPI;
using Core8WebAPI.Filters;
using FluentAssertions;
using FluentAssertions.Execution;
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

        var result = await InvokeEndpointFilterAsync();

        using (new AssertionScope())
        {
            _mockedLogger.DidNotReceiveWithAnyArgs().Log(Arg.Any<LogLevel>(), Arg.Any<EventId>(), Arg.Any<object?>(), Arg.Any<Exception>(), Arg.Any<Func<object?, Exception?, string>>());
            EndpointFilterContinued(result).Should().BeTrue();
        }
    }

    [Fact]
    public async Task LogsInfoWhenRequestResponseLoggingEnabled()
    {
        _settings.LogHttpRequestResponse = true;

        var result = await InvokeEndpointFilterAsync();

        using (new AssertionScope())
        {
            _mockedLogger.Received().Log(LogLevel.Information, Arg.Any<EventId>(), Arg.Any<object?>(), Arg.Any<Exception>(), Arg.Any<Func<object?, Exception?, string>>());
            EndpointFilterContinued(result).Should().BeTrue();
        }
    }
}
