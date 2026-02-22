using FluentAssertions;
using Xunit;
using ArchSim.Domain.Simulation;

namespace ArchSim.Domain.Tests;

public class ConnectionTests
{
    [Fact]
    public void Should_link_two_nodes()
    {
        var from = new SimulatedNode("FromNode", 10, 100, 100, 100);
        var to = new SimulatedNode("ToNode", 40, 100, 200, 200);

        var connection = new Connection(
            from,
            to,
            networkLatency: 5,
            timeout: 200
        );

        connection.From.Should().Be(from);
        connection.To.Should().Be(to);
    }

    [Fact]
    public void Should_expose_network_latency()
    {
        var from = new SimulatedNode("FromNode", 10, 100, 100, 100);
        var to = new SimulatedNode("ToNode", 40, 100, 200, 200);

        var connection = new Connection(from, to, 5, 50);

        connection.NetworkLatency.Should().Be(5);
    }
}