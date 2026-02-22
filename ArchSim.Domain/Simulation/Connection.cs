namespace ArchSim.Domain.Simulation;

public class Connection
{
    public ISimulatedNode From { get; }
    public ISimulatedNode To { get; }
    public double NetworkLatency { get; }
    public double Timeout { get; }

    public Connection(
        ISimulatedNode from,
        ISimulatedNode to,
        double networkLatency,
        double timeout)
    {
        From = from ?? throw new ArgumentNullException(nameof(from));
        To = to ?? throw new ArgumentNullException(nameof(to));

        if (timeout <= 0)
            throw new ArgumentException("Timeout must be greater than zero.", nameof(timeout));

        if (networkLatency < 0)
            throw new ArgumentException("Network latency cannot be negative.", nameof(networkLatency));

        NetworkLatency = networkLatency;
        Timeout = timeout;
    }
}