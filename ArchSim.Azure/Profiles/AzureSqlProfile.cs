using ArchSim.Azure.Catalog;
using ArchSim.Domain.Simulation;
using ArchSim.Domain.Simulation.Cost;

namespace ArchSim.Azure.Profiles;

public class AzureSqlProfile : AzureResourceProfile
{
    private const double DefaultTimeout = 2000;

    public AzureSqlProfile(string name, string sku)
        : base(name, sku)
    {
    }

    public override SimulatedNode ToSimulatedNode()
    {
        var (capacity, cost) = AzureSqlSkuCatalog.Resolve(Sku);

        return new SimulatedNode(
            label: Name,
            baseLatency: 20,
            capacity: capacity,
            timeout: DefaultTimeout,
            monthlyCost: cost,
            costPolicy: new FixedCostPolicy(cost));
    }
}