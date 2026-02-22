namespace ArchSim.Application.Contracts;

public class SimulationRequest
{
    public required CloudProvider Provider { get; init; }
    public required List<ResourceDefinition> Resources { get; init; }
    public required List<ConnectionDefinition> Connections { get; init; }
    public required double Load { get; init; }
}