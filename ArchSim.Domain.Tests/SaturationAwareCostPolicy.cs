using ArchSim.Domain.Simulation.Cost;

namespace ArchSim.Domain.Tests;

public class SaturationAwareCostPolicy : INodeCostPolicy
{
    public decimal CalculateMonthlyCost(
        double load,
        double capacity,
        bool isSaturated)
    {
        return isSaturated ? 2000 : 1000;
    }
}