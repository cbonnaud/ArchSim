namespace ArchSim.Application.Contracts;

public class SimulationResponse
{
    public double TotalLatency { get; init; }
    public bool HasErrors { get; init; }
    public decimal TotalMonthlyCost { get; init; }
    public decimal CostPerRequest { get; init; }
}