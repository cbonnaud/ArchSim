using ArchSim.Domain.Simulation;
using ArchSim.Domain.Simulation.Cost;
using FluentAssertions;

namespace ArchSim.Domain.Tests;

public class SimulatedNodeTests
{
    [Fact]
    public void Should_use_base_latency_when_utilization_is_less_or_equal_to_one()
    {
        var node = CreateNode(
            baseLatency: 10,
            capacity: 100);

        var result = node.Process(load: 50);

        result.Latency.Should().Be(10);
        result.IsSaturated.Should().BeFalse();
    }

    [Fact]
    public void Should_scale_latency_when_saturated()
    {
        var node = CreateNode(
            baseLatency: 10,
            capacity: 100);

        var result = node.Process(load: 200);

        result.Latency.Should().Be(20); // 10 * (200/100)
        result.IsSaturated.Should().BeTrue();
    }

    [Fact]
    public void Should_timeout_when_latency_equals_timeout()
    {
        var node = new SimulatedNode(
            "A",
            baseLatency: 100,
            capacity: 100,
            timeout: 100,
            monthlyCost: 100,
            costPolicy: new FixedCostPolicy(100));

        var result = node.Process(load: 100);

        result.HasTimedOut.Should().BeTrue();
    }

    [Fact]
    public void Should_handle_zero_load()
    {
        var node = CreateNode();

        var result = node.Process(0);

        result.Latency.Should().Be(10); // base latency
        result.IsSaturated.Should().BeFalse();
        result.HasTimedOut.Should().BeFalse();
    }

    private static SimulatedNode CreateNode(string label = "A", double baseLatency = 10, double capacity = 100)
    {
        return new SimulatedNode(
            label,
            baseLatency: baseLatency,
            capacity: capacity,
            timeout: 100,
            monthlyCost: 1000,
            costPolicy: new FixedCostPolicy(1000));
    }
}