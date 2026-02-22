namespace ArchSim.Domain.Simulation;

public class SimulationGraph
{
    public IReadOnlyCollection<SimulatedNode> Nodes { get; }
    public IReadOnlyCollection<Connection> Connections { get; }

    public SimulationGraph(
        IEnumerable<SimulatedNode> nodes,
        IEnumerable<Connection> connections)
    {
        if (nodes == null || !nodes.Any())
            throw new ArgumentException("Graph must contain at least one node.", nameof(nodes));

        if (connections == null)
            throw new ArgumentNullException(nameof(connections));

        var nodeSet = new HashSet<SimulatedNode>(nodes);
        foreach (var connection in connections)
        {
            if (!nodeSet.Contains(connection.From) || !nodeSet.Contains(connection.To))
                throw new ArgumentException("All connections must link existing nodes.", nameof(connections));
        }

        Nodes = nodeSet;
        Connections =[.. connections];
    }
}