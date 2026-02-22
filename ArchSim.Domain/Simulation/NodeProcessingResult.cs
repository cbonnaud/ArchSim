namespace ArchSim.Domain.Simulation;

public class NodeProcessingResult
{
    public double Latency { get; }
    public bool IsSaturated { get; }
    public bool HasTimedOut { get; }

    public NodeProcessingResult(double latency, bool isSaturated, bool hasTimedOut)
    {
        Latency = latency;
        IsSaturated = isSaturated;
        HasTimedOut = hasTimedOut;
    }
}