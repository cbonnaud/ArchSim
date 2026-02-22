namespace ArchSim.Domain.Simulation;

public class SimulatedNode(
    double baseLatency,
    double capacity,
    double timeout,
    decimal monthlyCost)
{
    public double BaseLatency { get; } = baseLatency;
    public double Capacity { get; } = capacity;
    public double Timeout { get; } = timeout;
    public decimal MonthlyCost { get; } = monthlyCost;

    public NodeProcessingResult Process(double load)
    {
        var utilization = load / Capacity;
        var latency = LatencyModel.Calculate(BaseLatency, utilization);
        var isSaturated = utilization > 1;
        var hasTimedOut = TimeoutPolicy.HasTimedOut(latency, Timeout);

        return new NodeProcessingResult(latency, isSaturated, hasTimedOut);
    }
}