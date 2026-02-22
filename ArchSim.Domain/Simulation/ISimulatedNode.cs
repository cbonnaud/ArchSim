namespace ArchSim.Domain.Simulation;

public interface ISimulatedNode
{
    string Label { get; }
    decimal MonthlyCost { get; }
    NodeProcessingResult Process(double load);
    decimal CalculateMonthlyCost(double load);
}