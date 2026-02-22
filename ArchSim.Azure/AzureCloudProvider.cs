using ArchSim.Application.Abstractions;
using ArchSim.Application.Contracts;
using ArchSim.Azure.Profiles;
using ArchSim.Domain.Simulation;

namespace ArchSim.Azure;

public class AzureCloudProvider : ICloudProvider
{
    public CloudProvider Provider => CloudProvider.Azure;

    public ISimulatedNode CreateNode(ResourceDefinition definition)
    {
        return definition.Type switch
        {
            "AppService" => new AzureAppServiceProfile(
                definition.Name,
                definition.Sku,
                definition.InstanceCount ?? 1)
                .ToSimulatedNode(),

            "Sql" => new AzureSqlProfile(
                definition.Name,
                definition.Sku)
                .ToSimulatedNode(),

            _ => throw new ArgumentException(
                $"Unsupported Azure resource type '{definition.Type}'.")
        };
    }

    public SimulationGraph BuildGraph(
        IEnumerable<ISimulatedNode> nodes,
        IEnumerable<ConnectionDefinition> connections)
    {
        var nodeList = nodes.ToList();
        var nodeMap = nodeList.ToDictionary(n => n.Label);

        var domainConnections = connections.Select(c =>
            new Connection(
                nodeMap[c.From],
                nodeMap[c.To],
                c.NetworkLatency,
                timeout: 5000));

        return new SimulationGraph(nodeList, domainConnections);
    }
}