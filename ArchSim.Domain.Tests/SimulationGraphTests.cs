using ArchSim.Domain.Simulation;
using ArchSim.Domain.Simulation.Cost;
using FluentAssertions;

namespace ArchSim.Domain.Tests;

public class SimulationGraphTests
{
    [Fact]
    public void Should_create_graph_with_nodes_and_connections()
    {
        var compute = new SimulatedNode("Compute", 10, 100, 100, 100, new FixedCostPolicy(100));
        var database = new SimulatedNode("Database", 40, 100, 200, 100, new FixedCostPolicy(100));

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

        graph.Nodes.Should().HaveCount(2);
        graph.Connections.Should().HaveCount(1);
    }

    [Fact]
    public void Constructor_Should_NotThrow_WhenGraphIsAcyclic()
    {
        var a = new SimulatedNode("A", 1, 1, 1, 10, new FixedCostPolicy(10));
        var b = new SimulatedNode("B", 1, 1, 1, 10, new FixedCostPolicy(10));
        var c = new SimulatedNode("C", 1, 1, 1, 10, new FixedCostPolicy(10));

        var connections = new[]
        {
            new Connection(a, b, 1, 100),
            new Connection(b, c, 1, 100)
        };

        var graph = new SimulationGraph(
            new[] { a, b, c },
            connections);

        Assert.NotNull(graph);
    }

    [Fact]
    public void Constructor_Should_Throw_WhenIndirectCycleExists()
    {
        var a = new SimulatedNode("A", 1, 1, 1, 10, new FixedCostPolicy(10));
        var b = new SimulatedNode("B", 1, 1, 1, 10, new FixedCostPolicy(10));
        var c = new SimulatedNode("C", 1, 1, 1, 10, new FixedCostPolicy(10));

        var connections = new[]
        {
            new Connection(a, b, 1, 100),
            new Connection(b, c, 1, 100),
            new Connection(c, a, 1, 100)
        };

        Assert.Throws<InvalidOperationException>(() =>
            new SimulationGraph(
                new[] { a, b, c },
                connections));
    }

    [Fact]
    public void Constructor_Should_NotThrow_WhenMultipleIndependentSubgraphsExist()
    {
        var a = new SimulatedNode("A", 1, 1, 1, 10, new FixedCostPolicy(10));
        var b = new SimulatedNode("B", 1, 1, 1, 10, new FixedCostPolicy(10));
        var c = new SimulatedNode("C", 1, 1, 1, 10, new FixedCostPolicy(10));
        var d = new SimulatedNode("D", 1, 1, 1, 10, new FixedCostPolicy(10));

        var connections = new[]
        {
            new Connection(a, b, 1, 100),
            new Connection(c, d, 1, 100)
        };

        var graph = new SimulationGraph(
            new[] { a, b, c, d },
            connections);

        Assert.NotNull(graph);
    }

    [Fact]
    public void Constructor_Should_Throw_WhenSelfReferenceExists()
    {
        var a = new SimulatedNode("A", 1, 1, 1, 10, new FixedCostPolicy(10));

        var connections = new[]
        {
            new Connection(a, a, 1, 100)
        };

        Assert.Throws<InvalidOperationException>(() =>
            new SimulationGraph(
                new[] { a },
                connections));
    }
}
