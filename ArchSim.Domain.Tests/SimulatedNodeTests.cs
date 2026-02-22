using FluentAssertions;
using Xunit;
using ArchSim.Domain.Simulation;

namespace ArchSim.Domain.Tests;

public class SimulatedNodeTests
{
    [Fact]
    public void Should_not_be_saturated_when_load_is_below_capacity()
    {
        var node = new SimulatedNode(
            "TestNode",
            baseLatency: 40,
            capacity: 100,
            timeout: 200,
            monthlyCost: 150
        );

        var result = node.Process(load: 40);

        result.IsSaturated.Should().BeFalse();
        result.Latency.Should().Be(40);
    }

    [Fact]
    public void Should_be_saturated_when_load_exceeds_capacity()
    {
        var node = new SimulatedNode(
            "TestNode",
            baseLatency: 40,
            capacity: 50,
            timeout: 200,
            monthlyCost: 150
        );

        var result = node.Process(load: 80);

        result.IsSaturated.Should().BeTrue();
        result.Latency.Should().Be(64); // 40 * 1.6
    }

    [Fact]
    public void Should_not_timeout_when_latency_is_below_timeout()
    {
        var node = new SimulatedNode(
            "TestNode",
            baseLatency: 40,
            capacity: 100,
            timeout: 200,
            monthlyCost: 150
        );

        var result = node.Process(load: 40);

        result.HasTimedOut.Should().BeFalse();
    }

    [Fact]
    public void Should_timeout_when_latency_exceeds_timeout()
    {
        var node = new SimulatedNode(
            "TestNode",
            baseLatency: 40,
            capacity: 50,
            timeout: 200,
            monthlyCost: 150
        );

        var result = node.Process(load: 300);
        // utilization = 6
        // latency = 240

        result.HasTimedOut.Should().BeTrue();
    }

    [Fact]
    public void Should_expose_monthly_cost()
    {
        var node = new SimulatedNode(
            "TestNode",
            baseLatency: 40,
            capacity: 100,
            timeout: 200,
            monthlyCost: 150
        );

        node.MonthlyCost.Should().Be(150);
    }
}