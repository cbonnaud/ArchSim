namespace ArchSim.Domain.Simulation.Cost;

public interface ICostModel
{
    public decimal CalculateTotalMonthlyCost(
        IReadOnlyCollection<ISimulatedNode> nodes,
        double load)
    {
        return nodes.Sum(n => n.CalculateMonthlyCost(load));
    }

    decimal CalculateCostPerRequest(
        decimal totalMonthlyCost,
        double load);
}