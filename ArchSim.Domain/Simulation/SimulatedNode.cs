using ArchSim.Domain.Simulation.Cost;

namespace ArchSim.Domain.Simulation;

public class SimulatedNode : ISimulatedNode
{
    public string Label { get; }
    public double BaseLatency { get; }
    public double Capacity { get; }
    public double Timeout { get; }
    public decimal MonthlyCost { get; }

    private readonly INodeCostPolicy _costPolicy;

    public SimulatedNode(
        string label,
        double baseLatency,
        double capacity,
        double timeout,
        decimal monthlyCost,
        INodeCostPolicy costPolicy)
    {
        if (baseLatency < 0)
            throw new ArgumentException("Base latency cannot be negative.", nameof(baseLatency));

        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));

        if (timeout <= 0)
            throw new ArgumentException("Timeout must be greater than zero.", nameof(timeout));

        if (monthlyCost < 0)
            throw new ArgumentException("Monthly cost cannot be negative.", nameof(monthlyCost));

        Label = label;
        BaseLatency = baseLatency;
        Capacity = capacity;
        Timeout = timeout;
        MonthlyCost = monthlyCost;
        _costPolicy = costPolicy;
    }

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