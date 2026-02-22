using FluentAssertions;
using Xunit;
using ArchSim.Domain.Simulation;

namespace ArchSim.Domain.Tests;

public class TimeoutPolicyTests
{
    [Fact]
    public void Should_not_timeout_when_latency_is_below_timeout()
    {
        var latency = 150;
        var timeout = 200;

        var hasTimedOut = TimeoutPolicy.HasTimedOut(latency, timeout);

        hasTimedOut.Should().BeFalse();
    }

    [Fact]
    public void Should_timeout_when_latency_exceeds_timeout()
    {
        var latency = 255;
        var timeout = 200;

        var hasTimedOut = TimeoutPolicy.HasTimedOut(latency, timeout);

        hasTimedOut.Should().BeTrue();
    }

    [Fact]
    public void Should_timeout_when_latency_reaches_timeout()
    {
        var latency = 200;
        var timeout = 200;

        var hasTimedOut = TimeoutPolicy.HasTimedOut(latency, timeout);

        hasTimedOut.Should().BeTrue();
    }
}