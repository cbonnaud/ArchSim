using ArchSim.Azure.Catalog;
using ArchSim.Domain.Simulation;
using ArchSim.Domain.Simulation.Cost;

namespace ArchSim.Azure.Profiles;

public class AzureAppServiceProfile : AzureResourceProfile
{
    public int InstanceCount { get; }

    public AzureAppServiceProfile(
        string name,
        string sku,
        int instanceCount)
        : base(name, sku)
    {
        if (instanceCount <= 0)
            throw new ArgumentException("Instance count must be greater than zero.", nameof(instanceCount));

        InstanceCount = instanceCount;
    }

    public override SimulatedNode ToSimulatedNode()
    {
        var (capacityPerInstance, costPerInstance) =
            AzureAppServiceSkuCatalog.Resolve(Sku);

        var totalCapacity = capacityPerInstance * InstanceCount;
        var totalCost = costPerInstance * InstanceCount;

        return new SimulatedNode(
            label: Name,
            baseLatency: 10,
            capacity: totalCapacity,
            timeout: 1000,
            monthlyCost: totalCost,
            costPolicy: new FixedCostPolicy(totalCost));
    }
}