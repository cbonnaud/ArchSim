namespace ArchSim.Domain.Simulation;

public static class LatencyModel
{
    public static double Calculate(double baseLatency, double utilization)
    {
        if (utilization <= 1)
            return baseLatency;

        return baseLatency * utilization;
    }
}