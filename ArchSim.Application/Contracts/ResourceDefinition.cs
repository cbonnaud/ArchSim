namespace ArchSim.Application.Contracts;

public class ResourceDefinition
{
    public required string Type { get; init; }
    public required string Name { get; init; }
    public required string Sku { get; init; }
    public int? InstanceCount { get; init; }
}