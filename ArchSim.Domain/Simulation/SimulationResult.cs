namespace ArchSim.Domain.Simulation;

public class SimulationResult(
    double totalLatency,
    bool hasErrors,
    decimal totalMonthlyCost,
    decimal costPerRequest)
{
    public double TotalLatency { get; } = totalLatency;
    public bool HasErrors { get; } = hasErrors;
    public decimal TotalMonthlyCost { get; } = totalMonthlyCost;
    public decimal CostPerRequest { get; } = costPerRequest;
}