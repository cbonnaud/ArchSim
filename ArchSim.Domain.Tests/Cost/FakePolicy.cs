using ArchSim.Domain.Simulation.Cost;

namespace ArchSim.Domain.Tests.Cost;

public class FakePolicy : INodeCostPolicy
{
    public double? ReceivedLoad;
    public double? ReceivedCapacity;
    public bool? ReceivedSaturation;

    public decimal CalculateMonthlyCost(
        double load,
        double capacity,
        bool isSaturated)
    {
        ReceivedLoad = load;
        ReceivedCapacity = capacity;
        ReceivedSaturation = isSaturated;

        return 42m;
    }
}