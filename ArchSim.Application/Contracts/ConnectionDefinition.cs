namespace ArchSim.Application.Contracts;

public class ConnectionDefinition
{
    public required string From { get; init; }
    public required string To { get; init; }
    public required double NetworkLatency { get; init; }
}