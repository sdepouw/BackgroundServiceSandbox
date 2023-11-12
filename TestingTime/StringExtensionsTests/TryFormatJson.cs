using Core7Library.Extensions;
using FluentAssertions;
using Xunit;
// ReSharper disable StringLiteralTypo

namespace TestingTime.StringExtensionsTests;

public class TryFormatJson
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ReturnsOriginalWhenEmpty(string emptyString)
    {
        string? formattedJson = emptyString.TryFormatJson();

        formattedJson.Should().Be(emptyString);
    }

    [Fact]
    public void ReturnsOriginalWhenNonJson()
    {
        const string nonJson = "Foo Bar Fizz Buzz";

        string? formattedJson = nonJson.TryFormatJson();

        formattedJson.Should().NotBeNull().And.Be(nonJson);
    }

    [Fact]
    public void FormatsJson()
    {
        const string unformattedJson = @"{""foo"":""Bob"",""bar"":1,""fizz"":[{""ace"":""win"",""king"":""lose""},{""queen"":""better"",""jack"":""worse""}]}";
        const string expectedFormattedJson =
@"{
  ""foo"": ""Bob"",
  ""bar"": 1,
  ""fizz"": [
    {
      ""ace"": ""win"",
      ""king"": ""lose""
    },
    {
      ""queen"": ""better"",
      ""jack"": ""worse""
    }
  ]
}";

        string? formattedJson = unformattedJson.TryFormatJson();

        formattedJson.Should().NotBeNull().And.Be(expectedFormattedJson);
    }
}
