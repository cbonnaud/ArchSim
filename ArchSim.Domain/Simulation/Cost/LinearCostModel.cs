namespace ArchSim.Domain.Simulation.Cost;

public class LinearCostModel : ICostModel
{
    public decimal CalculateTotalMonthlyCost(
        IReadOnlyCollection<ISimulatedNode> nodes,
        double load)
    {
        return nodes.Sum(n => n.CalculateMonthlyCost(load));
    }

    public decimal CalculateCostPerRequest(
        decimal totalMonthlyCost,
        double load)
    {
        if (load <= 0)
            return 0;

        // Hypothèse simple :
        // load = requests per second
        // 30 jours
        var monthlyRequests = (decimal)load * 60 * 60 * 24 * 30;

        return totalMonthlyCost / monthlyRequests;
    }
}