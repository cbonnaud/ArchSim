namespace ArchSim.Domain.Simulation;

public static class TimeoutPolicy
{
    public static bool HasTimedOut(double latency, double timeout)
    {
        return latency >= timeout;
    }
}