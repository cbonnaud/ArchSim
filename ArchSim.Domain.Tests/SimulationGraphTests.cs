// namespace ArchSim.Domain.Tests;

// public class SimulationGraphTests
// {
//     [Fact]
//     public void Should_create_graph_with_nodes_and_connections()
//     {
//         var compute = new SimulatedNode(10, 100, 100);
//         var database = new SimulatedNode(40, 100, 200);

//         var connection = new Connection(
//             compute,
//             database,
//             networkLatency: 5,
//             timeout: 200
//         );

//         var graph = new SimulationGraph(
//             new[] { compute, database },
//             new[] { connection }
//         );

//         graph.Nodes.Should().HaveCount(2);
//         graph.Connections.Should().HaveCount(1);
//     }
// }
