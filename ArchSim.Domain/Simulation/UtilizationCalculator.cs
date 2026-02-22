namespace ArchSim.Domain.Simulation;

public static class UtilizationCalculator
{
    public static double Calculate(double load, double capacity)
    {
        if (capacity == 0)
        {
            throw new ArgumentException("Capacity cannot be zero.", nameof(capacity));
        }

        return load / capacity;
    }
}