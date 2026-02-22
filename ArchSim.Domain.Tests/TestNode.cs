using ArchSim.Domain.Simulation;

namespace ArchSim.Domain.Tests;

public class TestNode(string label, double baseLatency, double capacity, decimal cost) : ISimulatedNode
{
    public int ProcessCallCount { get; private set; }

    public string Label { get; } = label;
    public double Capacity { get; } = capacity;
    public decimal MonthlyCost { get; } = cost;

    public decimal CalculateMonthlyCost(double load)
    {
        throw new NotImplementedException();
    }

    public NodeProcessingResult Process(double load)
    {
        ProcessCallCount++;
        return new NodeProcessingResult(baseLatency, false, false);
    }
}