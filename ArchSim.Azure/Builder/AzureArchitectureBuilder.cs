using ArchSim.Domain.Simulation;
using ArchSim.Azure.Profiles;

namespace ArchSim.Azure.Builder;

public class AzureArchitectureBuilder
{
    private readonly List<AzureResourceProfile> _resources = new();
    private readonly List<(AzureResourceProfile from, AzureResourceProfile to, double latency)> _links = new();

    public AzureArchitectureBuilder AddResource(AzureResourceProfile resource)
    {
        _resources.Add(resource);
        return this;
    }

    public AzureArchitectureBuilder Connect(
        AzureResourceProfile from,
        AzureResourceProfile to,
        double networkLatency)
    {
        _links.Add((from, to, networkLatency));
        return this;
    }

    public SimulationGraph Build()
    {
        var nodes = _resources
            .Select(r => r.ToSimulatedNode())
            .ToList();

        var nodeMap = nodes.ToDictionary(n => n.Label);

        var connections = _links
            .Select(link =>
                new Connection(
                    nodeMap[link.from.Name],
                    nodeMap[link.to.Name],
                    link.latency,
                    timeout: 5000))
            .ToList();

        return new SimulationGraph(nodes, connections);
    }
}