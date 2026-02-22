namespace ArchSim.Cloud.Models;

public class CloudResourceProfile
{
    public string ResourceType { get; init; } = default!;
    public string Sku { get; init; } = default!;

    public PerformanceProfile Performance { get; init; } = default!;
    public CapacityProfile Capacity { get; init; } = default!;
    public CostProfile Cost { get; init; } = default!;
}