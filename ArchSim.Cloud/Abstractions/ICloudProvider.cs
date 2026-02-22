namespace ArchSim.Cloud.Abstractions;

using ArchSim.Application.Contracts;
using ArchSim.Cloud.Models;
using ArchSim.Domain.Simulation;

public interface ICloudProvider
{
    CloudProviderType ProviderType { get; }

    ISimulatedNode CreateNode(ResourceDefinition definition);

    SimulationGraph BuildGraph(
        IEnumerable<ISimulatedNode> nodes,
        IEnumerable<Connection> connections);
}