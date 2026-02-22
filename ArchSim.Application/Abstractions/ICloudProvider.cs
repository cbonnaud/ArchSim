using ArchSim.Application.Contracts;
using ArchSim.Domain.Simulation;

namespace ArchSim.Application.Abstractions;

public interface ICloudProvider
{
    CloudProvider Provider { get; }

    ISimulatedNode CreateNode(ResourceDefinition definition);

    SimulationGraph BuildGraph(
        IEnumerable<ISimulatedNode> nodes,
        IEnumerable<ConnectionDefinition> connections);
}