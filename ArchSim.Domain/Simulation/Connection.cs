namespace ArchSim.Domain.Simulation;

public class Connection
{
    public SimulatedNode From { get; }
    public SimulatedNode To { get; }
    public double NetworkLatency { get; }
    public double Timeout { get; }

    public Connection(
        SimulatedNode from,
        SimulatedNode to,
        double networkLatency,
        double timeout)
    {
        From = from ?? throw new ArgumentNullException(nameof(from));
        To = to ?? throw new ArgumentNullException(nameof(to));
        NetworkLatency = networkLatency;
        Timeout = timeout;
    }
}