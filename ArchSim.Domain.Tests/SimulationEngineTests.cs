using FluentAssertions;
using Xunit;
using ArchSim.Domain.Simulation;

namespace ArchSim.Domain.Tests;

public class SimulationEngineTests
{
    // [Fact]
    // public void Should_calculate_total_latency_without_errors_when_not_saturated()
    // {
    //     var compute = new SimulatedNode("Compute", 10, 100, 200, 150);
    //     var database = new SimulatedNode("Database", 40, 100, 200, 200);

    //     var engine = new SimulationEngine([compute, database]);

    //     var result = engine.Run(load: 40);

    //     result.TotalLatency.Should().Be(50); // 10 + 40
    //     result.HasErrors.Should().BeFalse();
    // }

    // [Fact]
    // public void Should_report_error_when_database_times_out()
    // {
    //     var compute = new SimulatedNode("Compute", 10, 100, 200, 150);
    //     var database = new SimulatedNode("Database", 40, 50, 200, 200);

    //     var engine = new SimulationEngine([compute, database]);

    //     var result = engine.Run(load: 300);

    //     result.HasErrors.Should().BeTrue();
    // }

    // [Fact]
    // public void Should_process_multiple_nodes_in_sequence()
    // {
    //     var compute = new SimulatedNode("Compute", 10, 100, 200, 150);
    //     var database = new SimulatedNode("Database", 40, 100, 200, 200);
    //     var cache = new SimulatedNode("Cache", 5, 100, 200, 100);

    //     var engine = new SimulationEngine([compute, cache, database]);

    //     var result = engine.Run(load: 40);

    //     result.TotalLatency.Should().Be(55);
    //     // 10 + 5 + 40

    //     result.HasErrors.Should().BeFalse();
    // }

    // [Fact]
    // public void Should_calculate_total_monthly_cost()
    // {
    //     var compute = new SimulatedNode("Compute", 10, 100, 200, 100);
    //     var database = new SimulatedNode("Database", 40, 100, 200, 200);

    //     var engine = new SimulationEngine([compute, database]);

    //     var result = engine.Run(load: 40);

    //     result.TotalMonthlyCost.Should().Be(300);
    // }

    // [Fact]
    // public void Should_calculate_cost_per_request()
    // {
    //     var compute = new SimulatedNode("Compute", 10, 100, 200, 100);
    //     var database = new SimulatedNode("Database", 40, 100, 200, 200);

    //     var engine = new SimulationEngine([compute, database]);

    //     var result = engine.Run(load: 10);

    //     result.CostPerRequest.Should().BeGreaterThan(0);
    // }

    [Fact]
    public void Should_calculate_total_latency_with_network_latency()
    {
        var compute = new SimulatedNode("Compute", 10, 100, 100, 150);
        var database = new SimulatedNode("Database", 40, 100, 200, 150);

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

        var engine = new SimulationEngine(graph);

        var result = engine.Run(load: 40);

        result.TotalLatency.Should().Be(55);
        // 10 + 5 + 40

        result.HasErrors.Should().BeFalse();
        result.TotalMonthlyCost.Should().Be(300);
        result.CostPerRequest.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_report_error_when_connection_timeout_is_exceeded()
    {
        var compute = new SimulatedNode("Compute", 10, 100, 100, 150);
        var database = new SimulatedNode("Database", 40, 50, 200, 150);

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

        var engine = new SimulationEngine(graph);

        var result = engine.Run(load: 300);

        result.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void Run_Should_Not_Duplicate_MonthlyCost_When_Node_Converges()
    {
        var a = new SimulatedNode("A", 1, 10, 100, 10);
        var b = new SimulatedNode("B", 1, 10, 100, 10);
        var c = new SimulatedNode("C", 1, 10, 100, 10);
        var d = new SimulatedNode("D", 1, 10, 100, 10);

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

        var engine = new SimulationEngine(graph);

        var result = engine.Run(5);

        Assert.Equal(40m, result.TotalMonthlyCost);
    }
}