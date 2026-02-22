using FluentAssertions;
using Xunit;
using ArchSim.Domain.Simulation;
using ArchSim.Domain.Tests.Cost;

namespace ArchSim.Domain.Tests;

public class SimulationEngineTests
{
    [Fact]
    public void Should_calculate_total_latency_with_network_latency()
    {
        var fakePolicy = new FakePolicy();

        var compute = new SimulatedNode("Compute", 10, 100, 100, 150, fakePolicy);
        var database = new SimulatedNode("Database", 40, 100, 200, 150, fakePolicy);

        var connection = new Connection(
            compute,
            database,
            networkLatency: 5,
            timeout: 200
        );

        var graph = new SimulationGraph(
            new[] { compute, database },
            new[] { connection }
        );

        var engine = new SimulationEngine(graph, new FakeCostModel());

        var result = engine.Run(load: 40);

        result.TotalLatency.Should().Be(55);
        // 10 + 5 + 40

        result.HasErrors.Should().BeFalse();
        result.TotalMonthlyCost.Should().Be(84);
        result.CostPerRequest.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_report_error_when_connection_timeout_is_exceeded()
    {
        var fakePolicy = new FakePolicy();

        var compute = new SimulatedNode("Compute", 10, 100, 100, 150, fakePolicy);
        var database = new SimulatedNode("Database", 40, 50, 200, 150, fakePolicy);

        var connection = new Connection(
            compute,
            database,
            networkLatency: 5,
            timeout: 50
        );

        var graph = new SimulationGraph(
            new[] { compute, database },
            new[] { connection }
        );

        var engine = new SimulationEngine(graph, new FakeCostModel());

        var result = engine.Run(load: 300);

        result.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void Run_Should_Not_Duplicate_MonthlyCost_When_Node_Converges()
    {
        var fakePolicy = new FakePolicy();

        var a = new SimulatedNode("A", 1, 10, 100, 10, fakePolicy);
        var b = new SimulatedNode("B", 1, 10, 100, 10, fakePolicy);
        var c = new SimulatedNode("C", 1, 10, 100, 10, fakePolicy);
        var d = new SimulatedNode("D", 1, 10, 100, 10, fakePolicy);

        var connections = new[]
        {
            new Connection(a, b, 1, 100),
            new Connection(a, c, 1, 100),
            new Connection(b, d, 1, 100),
            new Connection(c, d, 1, 100)
        };

        var graph = new SimulationGraph(
            new[] { a, b, c, d },
            connections);

        var engine = new SimulationEngine(graph, new FakeCostModel());

        var result = engine.Run(5);

        Assert.Equal(4 * 42, result.TotalMonthlyCost);
    }

    [Fact]
    public void Run_Should_Sum_All_Node_Costs_Linearly()
    {
        var fakePolicy = new FakePolicy();

        var a = new SimulatedNode("A", 1, 10, 100, 10, fakePolicy);
        var b = new SimulatedNode("B", 1, 10, 100, 20, fakePolicy);
        var c = new SimulatedNode("C", 1, 10, 100, 30, fakePolicy);

        var graph = new SimulationGraph(
            new[] { a, b, c },
            Array.Empty<Connection>());

        var engine = new SimulationEngine(graph, new FakeCostModel());

        var result = engine.Run(5);

        Assert.Equal(3 * 42, result.TotalMonthlyCost);
    }
}