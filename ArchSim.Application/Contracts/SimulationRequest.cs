using ArchSim.Cloud.Models;
using ArchSim.Domain.Simulation;

namespace ArchSim.Application.Contracts;

public class SimulationRequest
{
    public required CloudProviderType Provider { get; init; }
    public required List<ResourceDefinition> Resources { get; init; }
    public required List<Connection> Connections { get; init; }
    public required double Load { get; init; }
}