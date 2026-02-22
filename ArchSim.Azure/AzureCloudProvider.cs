using ArchSim.Application.Contracts;
using ArchSim.Azure.Catalog;
using ArchSim.Cloud.Abstractions;
using ArchSim.Cloud.Models;
using ArchSim.Domain.Simulation;
using ArchSim.Domain.Simulation.Cost;

namespace ArchSim.Azure;

public class AzureCloudProvider : ICloudProvider
{
    public const string Azure = "Azure";

    private readonly AzureCatalog _catalog = new();

    public CloudProviderType ProviderType => CloudProviderType.Azure;

    public ISimulatedNode CreateNode(ResourceDefinition definition)
    {
        var profile = _catalog.GetProfile(definition.Type, definition.Sku);

        var totalCapacity = profile.Capacity.RequestsPerSecond
                            * (definition.InstanceCount ?? 1);

        var totalCost = profile.Cost.MonthlyCost
                        * (definition.InstanceCount ?? 1);

        return new SimulatedNode(
            label: definition.Name,
            baseLatency: profile.Performance.BaseLatencyMs,
            capacity: totalCapacity,
            timeout: profile.Performance.TimeoutMs,
            monthlyCost: totalCost,
            costPolicy: new FixedCostPolicy(totalCost));
    }

    public SimulationGraph BuildGraph(
        IEnumerable<ISimulatedNode> nodes,
        IEnumerable<Connection> connections)
    {
        var nodeList = nodes.ToList();
        var nodeMap = nodeList.ToDictionary(n => n.Label);

        var domainConnections = connections.Select(c =>
            new Connection(
                nodeMap[c.From.Label],
                nodeMap[c.To.Label],
                c.NetworkLatency,
                timeout: 5000));

        return new SimulationGraph(nodeList, domainConnections);
    }
}