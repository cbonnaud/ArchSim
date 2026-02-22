using ArchSim.Domain.Simulation.Cost;

namespace ArchSim.Domain.Simulation;

public class SimulatedNode(
    string label,
    double baseLatency,
    double capacity,
    double timeout,
    decimal monthlyCost,
    INodeCostPolicy costPolicy) : ISimulatedNode
{
    public string Label { get; } = label;
    public double BaseLatency { get; } = baseLatency;
    public double Capacity { get; } = capacity;
    public double Timeout { get; } = timeout;
    public decimal MonthlyCost { get; } = monthlyCost;

    private readonly INodeCostPolicy _costPolicy = costPolicy;

    public NodeProcessingResult Process(double load)
    {
        var utilization = load / Capacity;
        var latency = LatencyModel.Calculate(BaseLatency, utilization);
        var isSaturated = utilization > 1;
        var hasTimedOut = TimeoutPolicy.HasTimedOut(latency, Timeout);

        return new NodeProcessingResult(latency, isSaturated, hasTimedOut);
    }

    public decimal CalculateMonthlyCost(double load)
    {
        var utilization = load / Capacity;
        var isSaturated = utilization > 1;

        return _costPolicy.CalculateMonthlyCost(
            load,
            Capacity,
            isSaturated);
    }
}