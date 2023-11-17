using Core7Library;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace TestingTime;

public class Time
{
    [Fact]
    public void ItDistractsFromTheNow()
    {
        FakeTimeProvider fake = new FakeTimeProvider();
        FakeTimeProviderAbstraction abs = new();

        TimeProvider provider = TimeProvider.System;

        DateTimeOffset localNow = provider.GetLocalNow();

        localNow.Should().Be(localNow.UtcDateTime);
    }
}

/// <summary>
/// Fake <see cref="TimeProviderAbstraction" /> whose implementation uses .NET 8's
/// <see cref="FakeTimeProvider" /> in Microsoft.Extensions.TimeProvider.Testing NuGet package
/// under the hood
/// </summary>
public class FakeTimeProviderAbstraction : TimeProviderAbstraction
{
    private readonly FakeTimeProvider _fake = new();

    public override DateTimeOffset GetLocalNow() => _fake.GetLocalNow();
    public override DateTimeOffset GetUtcNow() => _fake.GetUtcNow();
    /// <inheritdoc cref="FakeTimeProvider.SetUtcNow" />
    public void SetUtcNow(DateTimeOffset value) => _fake.SetUtcNow(value);
}
