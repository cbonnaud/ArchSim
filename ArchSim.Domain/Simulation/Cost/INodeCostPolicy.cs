namespace ArchSim.Domain.Simulation.Cost;

public interface INodeCostPolicy
{
    decimal CalculateMonthlyCost(
        double load,
        double capacity,
        bool isSaturated);
}