using FluentAssertions;
using Xunit;
using ArchSim.Domain.Simulation;

namespace ArchSim.Domain.Tests;

public class LatencyModelTests
{
    [Fact]
    public void Should_return_base_latency_when_not_saturated()
    {
        var baseLatency = 40;
        var utilization = 0.8;

        var latency = LatencyModel.Calculate(baseLatency, utilization);

        latency.Should().Be(40);
    }

    [Fact]
    public void Should_increase_latency_proportionally_when_saturated()
    {
        var baseLatency = 40;
        var utilization = 1.6;

        var latency = LatencyModel.Calculate(baseLatency, utilization);

        latency.Should().Be(64);
    }
}