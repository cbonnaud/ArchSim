using ArchSim.Domain.Simulation;
using ArchSim.Domain.Simulation.Cost;
using FluentAssertions;

namespace ArchSim.Domain.Tests;

public class SimulationGraphTests
{
    [Fact]
    public void Should_throw_when_connection_references_unknown_node()
    {
        var nodeA = CreateNode("A");
        var nodeB = CreateNode("B");

        var externalNode = CreateNode("X");

        var connection = new Connection(nodeA, externalNode, 1, 10);

        var nodes = new[] { nodeA, nodeB };
        var connections = new[] { connection };

        Assert.Throws<InvalidOperationException>(() =>
            new SimulationGraph(nodes, connections));
    }

    [Fact]
    public void Should_throw_when_graph_contains_simple_cycle()
    {
        var nodeA = CreateNode("A");
        var nodeB = CreateNode("B");

        var connections = new[]
        {
            new Connection(nodeA, nodeB, 1, 10),
            new Connection(nodeB, nodeA, 1, 10)
        };

        Assert.Throws<InvalidOperationException>(() =>
            new SimulationGraph(new[] { nodeA, nodeB }, connections));
    }

    [Fact]
    public void Should_throw_when_graph_has_multiple_roots()
    {
        var nodeA = CreateNode("A");
        var nodeB = CreateNode("B");

        var nodes = new[] { nodeA, nodeB };
        var connections = Array.Empty<Connection>();

        Assert.Throws<InvalidOperationException>(() =>
            new SimulationGraph(nodes, connections));
    }

    [Fact]
    public void Should_allow_single_node_without_connections()
    {
        var nodeA = CreateNode("A");

        var graph = new SimulationGraph(
            new[] { nodeA },
            Array.Empty<Connection>());

        graph.Nodes.Should().HaveCount(1);
    }

    [Fact]
    public void Should_simulate_single_node_graph()
    {
        var node = CreateNode(
            label: "A",
            baseLatency: 10,
            capacity: 100,
            timeout: 50,
            monthlyCost: 1000);

        var graph = new SimulationGraph(
            new[] { node },
            Array.Empty<Connection>());

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(load: 50);

        result.TotalLatency.Should().Be(10);
        result.HasErrors.Should().BeFalse();
        result.TotalMonthlyCost.Should().Be(1000);
    }

    [Fact]
    public void Should_simulate_linear_chain()
    {
        var nodeA = CreateNode("A", 10);
        var nodeB = CreateNode("B", 20);

        var connection = new Connection(nodeA, nodeB, 5, 100);

        var graph = new SimulationGraph(
            new[] { nodeA, nodeB },
            new[] { connection });

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(load: 50);

        result.TotalLatency.Should().Be(35);
        result.HasErrors.Should().BeFalse();
    }

    [Fact]
    public void Should_take_max_latency_for_parallel_branches()
    {
        var nodeA = CreateNode("A", 10);
        var nodeB = CreateNode("B", 20);
        var nodeC = CreateNode("C", 40);

        var connections = new[]
        {
        new Connection(nodeA, nodeB, 5, 100),
        new Connection(nodeA, nodeC, 5, 100)
    };

        var graph = new SimulationGraph(
            new[] { nodeA, nodeB, nodeC },
            connections);

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(load: 50);

        result.TotalLatency.Should().Be(55);
        result.HasErrors.Should().BeFalse();
    }

    [Fact]
    public void Should_correctly_handle_converging_dag()
    {
        var nodeA = CreateNode("A", 10);
        var nodeB = CreateNode("B", 20);
        var nodeC = CreateNode("C", 30);
        var nodeD = CreateNode("D", 40);

        var connections = new[]
        {
            new Connection(nodeA, nodeB, 5, 500),
            new Connection(nodeA, nodeC, 5, 500),
            new Connection(nodeB, nodeD, 5, 500),
            new Connection(nodeC, nodeD, 5, 500)
        };

        var graph = new SimulationGraph(
            new[] { nodeA, nodeB, nodeC, nodeD },
            connections);

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(load: 50);

        result.TotalLatency.Should().Be(90);
        result.HasErrors.Should().BeFalse();
    }

    [Fact]
    public void Should_flag_error_even_if_timeout_occurs_on_non_dominant_branch()
    {
        var nodeA = CreateNode("A", 10);
        var nodeB = CreateNode("B", 20);
        var nodeC = CreateNode("C", 30);
        var nodeD = CreateNode("D", 40);

        var connections = new[]
        {
            new Connection(nodeA, nodeB, 5, 500),
            new Connection(nodeA, nodeC, 5, 500),
            new Connection(nodeB, nodeD, 5, 40),  // timeout here (45 >= 40)
            new Connection(nodeC, nodeD, 5, 500)
        };

        var graph = new SimulationGraph(
            new[] { nodeA, nodeB, nodeC, nodeD },
            connections);

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(load: 50);

        result.TotalLatency.Should().Be(90);
        result.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void Should_propagate_saturation_correctly_across_deep_graph()
    {
        var nodeA = CreateNode("A", 10, timeout: 1000);
        var nodeB = CreateNode("B", 20, timeout: 1000);
        var nodeC = CreateNode("C", 30, timeout: 1000);
        var nodeD = CreateNode("D", 40, timeout: 1000);

        var connections = new[]
        {
            new Connection(nodeA, nodeB, 5, 1000),
            new Connection(nodeA, nodeC, 5, 1000),
            new Connection(nodeB, nodeD, 5, 1000),
            new Connection(nodeC, nodeD, 5, 1000)
        };

        var graph = new SimulationGraph(
            new[] { nodeA, nodeB, nodeC, nodeD },
            connections);

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(load: 300);

        result.TotalLatency.Should().Be(250);
        result.HasErrors.Should().BeFalse();
    }

    [Fact]
    public void Should_handle_asymmetrical_graph_correctly()
    {
        var nodeA = CreateNode("A", 5);
        var nodeB = CreateNode("B", 5);
        var nodeC = CreateNode("C", 40);
        var nodeD = CreateNode("D", 5);
        var nodeE = CreateNode("E", 5);

        var connections = new[]
        {
            new Connection(nodeA, nodeB, 2, 1000),
            new Connection(nodeB, nodeD, 2, 1000),
            new Connection(nodeD, nodeE, 2, 1000),

            new Connection(nodeA, nodeC, 2, 1000)
        };

        var graph = new SimulationGraph(
            new[] { nodeA, nodeB, nodeC, nodeD, nodeE },
            connections);

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(load: 50);

        // Long branch:
        // E=5
        // D=5+(2+5)=12
        // B=5+(2+12)=19
        // A branch1=2+19=21

        // C branch:
        // C=40
        // A branch2=2+40=42

        // Max = 42
        // Total = 5 + 42 = 47

        result.TotalLatency.Should().Be(47);
    }

    [Fact]
    public void Should_handle_complex_multi_level_graph()
    {
        var A = CreateNode("A", 10);
        var B = CreateNode("B", 10);
        var C = CreateNode("C", 20);
        var D = CreateNode("D", 10);
        var E = CreateNode("E", 30);
        var F = CreateNode("F", 15);
        var G = CreateNode("G", 25);

        var connections = new[]
        {
            new Connection(A, B, 5, 1000),
            new Connection(A, C, 5, 1000),

            new Connection(B, D, 5, 1000),
            new Connection(B, E, 5, 1000),

            new Connection(E, G, 5, 1000),
            new Connection(C, F, 5, 1000),
            new Connection(F, G, 5, 1000)
        };

        var graph = new SimulationGraph(
            new[] { A, B, C, D, E, F, G },
            connections);

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(50);

        result.HasErrors.Should().BeFalse();
        result.TotalLatency.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_use_node_cost_policy_based_on_saturation()
    {
        var node = new SimulatedNode(
            "A",
            baseLatency: 10,
            capacity: 100,
            timeout: 100,
            monthlyCost: 0,
            costPolicy: new SaturationAwareCostPolicy());

        var graph = new SimulationGraph(
            new[] { node },
            Array.Empty<Connection>());

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(load: 200);

        result.TotalMonthlyCost.Should().Be(2000);
    }

    [Fact]
    public void Should_handle_zero_load_on_complex_graph()
    {
        var A = CreateNode("A", 10);
        var B = CreateNode("B", 20);
        var C = CreateNode("C", 30);

        var connections = new[]
        {
            new Connection(A, B, 5, 1000),
            new Connection(A, C, 5, 1000)
        };

        var graph = new SimulationGraph(
            new[] { A, B, C },
            connections);

        var engine = new SimulationEngine(graph, new LinearCostModel());

        var result = engine.Run(0);

        result.TotalLatency.Should().Be(10 + Math.Max(5 + 20, 5 + 30));
        result.CostPerRequest.Should().Be(0);
    }

    private static SimulatedNode CreateNode(
        string label,
        double baseLatency = 10,
        double capacity = 100,
        double timeout = 100,
        decimal monthlyCost = 1000)
    {
        return new SimulatedNode(
            label,
            baseLatency: baseLatency,
            capacity: capacity,
            timeout: timeout,
            monthlyCost: monthlyCost,
            costPolicy: new FixedCostPolicy(monthlyCost));
    }
}