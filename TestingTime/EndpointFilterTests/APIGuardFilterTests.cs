using Core8WebAPI;
using Core8WebAPI.Filters;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace TestingTime.EndpointFilterTests;

public class APIGuardFilterTests : EndpointFilterTestBase
{
    private readonly ExampleSettings _settings = new() { APISecretKey = "def-456" };
    private readonly APIGuardFilter _filter;
    protected override IEndpointFilter EndpointFilter => _filter;

    public APIGuardFilterTests() => _filter = new APIGuardFilter(Options.Create(_settings));

    [Fact]
    public async Task ContinuesWhenAPIKeyMatches()
    {
        AppendRequestHeader("Secret", _settings.APISecretKey);

        var result = await InvokeEndpointFilterAsync();

        EndpointFilterContinued(result).Should().BeTrue();
    }

    [Fact]
    public async Task ReturnsNotFoundForMissingHeader()
    {
        IResult expectedResult = Results.NotFound();

        var result = await InvokeEndpointFilterAsync();

        using (new AssertionScope())
        {
            result.Should().Be(expectedResult);
            EndpointFilterContinued(result).Should().BeFalse();
        }
    }

    [Fact]
    public async Task ReturnsNotFoundForHeaderWithWrongAPIKey()
    {
        IResult expectedResult = Results.NotFound();
        AppendRequestHeader("Secret", "incorrect-api-key");

        var result = await InvokeEndpointFilterAsync();

        using (new AssertionScope())
        {
            result.Should().Be(expectedResult);
            EndpointFilterContinued(result).Should().BeFalse();
        }
    }
}
