namespace ArchSim.Domain.Simulation.Cost;

public class FixedCostPolicy : INodeCostPolicy
{
    private readonly decimal _monthlyCost;

    public FixedCostPolicy(decimal monthlyCost)
    {
        _monthlyCost = monthlyCost;
    }

    public decimal CalculateMonthlyCost(
        double load,
        double capacity,
        bool isSaturated)
        => _monthlyCost;
}