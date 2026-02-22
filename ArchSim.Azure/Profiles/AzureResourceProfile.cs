using ArchSim.Domain.Simulation;

namespace ArchSim.Azure.Profiles;

public abstract class AzureResourceProfile
{
    public string Name { get; }
    public string Sku { get; }

    protected AzureResourceProfile(string name, string sku)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Sku = sku ?? throw new ArgumentNullException(nameof(sku));
    }

    public abstract SimulatedNode ToSimulatedNode();
}