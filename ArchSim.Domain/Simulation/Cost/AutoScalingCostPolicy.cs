namespace ArchSim.Domain.Simulation.Cost;

public class AutoScalingCostPolicy : INodeCostPolicy
{
    public decimal CalculateMonthlyCost(
        double load,
        double capacity,
        bool isSaturated)
    {
        // var requiredInstances = Math.Ceiling(load / capacity);
        // return requiredInstances * _instanceCost;
        throw new NotImplementedException("Auto-scaling cost policy is not implemented yet.");
    }
}