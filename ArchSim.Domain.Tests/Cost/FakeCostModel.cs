using ArchSim.Domain.Simulation.Cost;

namespace ArchSim.Domain.Tests;

public class FakeCostModel : ICostModel
{
    public decimal CalculateCostPerRequest(decimal totalMonthlyCost, double load)
    {
        return 42m;
    }
}