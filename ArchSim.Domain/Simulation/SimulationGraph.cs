namespace ArchSim.Domain.Simulation;

public class SimulationGraph
{
    public IReadOnlyCollection<ISimulatedNode> Nodes { get; }
    public IReadOnlyCollection<Connection> Connections { get; }

    private readonly Dictionary<ISimulatedNode, List<Connection>> _adjacency;

    public SimulationGraph(
        IEnumerable<ISimulatedNode> nodes,
        IEnumerable<Connection> connections)
    {
        Nodes = nodes.ToList().AsReadOnly();
        Connections = connections.ToList().AsReadOnly();

        ValidateConnections();
        _adjacency = BuildAdjacencyDictionary();
        ValidateAcyclic();
    }

    public IReadOnlyList<Connection> GetOutgoingConnections(ISimulatedNode node)
    {
        if (!_adjacency.TryGetValue(node, out var connections))
            throw new InvalidOperationException("Node not found in graph.");

        return connections;
    }

    private void ValidateConnections()
    {
        var nodeSet = Nodes.ToHashSet();

        foreach (var connection in Connections)
        {
            if (!nodeSet.Contains(connection.From) ||
                !nodeSet.Contains(connection.To))
            {
                throw new InvalidOperationException(
                    "Connection references unknown node.");
            }
        }
    }

    private Dictionary<ISimulatedNode, List<Connection>> BuildAdjacencyDictionary()
    {
        var dict = Nodes.ToDictionary(n => n, _ => new List<Connection>());

        foreach (var connection in Connections)
        {
            dict[connection.From].Add(connection);
        }

        return dict;
    }

    private void ValidateAcyclic()
    {
        var visited = new HashSet<ISimulatedNode>();
        var visiting = new HashSet<ISimulatedNode>();

        foreach (var node in Nodes)
        {
            if (!visited.Contains(node))
            {
                DFS(node, visited, visiting);
            }
        }
    }

    private void DFS(
        ISimulatedNode node,
        HashSet<ISimulatedNode> visited,
        HashSet<ISimulatedNode> visiting)
    {
        visiting.Add(node);

        foreach (var connection in _adjacency[node])
        {
            var next = connection.To;

            if (visiting.Contains(next))
            {
                throw new InvalidOperationException(
                    $"Cycle detected involving node '{next}'.");
            }

            if (!visited.Contains(next))
            {
                DFS(next, visited, visiting);
            }
        }

        visiting.Remove(node);
        visited.Add(node);
    }
}