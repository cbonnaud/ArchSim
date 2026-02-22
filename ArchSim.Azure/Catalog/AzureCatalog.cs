using ArchSim.Cloud.Abstractions;
using ArchSim.Cloud.Models;

namespace ArchSim.Azure.Catalog;

public class AzureCatalog : ICloudCatalog
{
    private readonly Dictionary<(string, string), CloudResourceProfile> _profiles;

    public AzureCatalog()
    {
        _profiles = new()
        {
            {
                ("AppService", "B1"),
                new CloudResourceProfile
                {
                    ResourceType = "AppService",
                    Sku = "B1",
                    Performance = new PerformanceProfile
                    {
                        BaseLatencyMs = 20,
                        TimeoutMs = 1000
                    },
                    Capacity = new CapacityProfile
                    {
                        RequestsPerSecond = 100
                    },
                    Cost = new CostProfile
                    {
                        MonthlyCost = 55m
                    }
                }
            },
            {
                ("AppService", "P1V2"),
                new CloudResourceProfile
                {
                    ResourceType = "AppService",
                    Sku = "P1V2",
                    Performance = new PerformanceProfile
                    {
                        BaseLatencyMs = 10,
                        TimeoutMs = 1000
                    },
                    Capacity = new CapacityProfile
                    {
                        RequestsPerSecond = 210
                    },
                    Cost = new CostProfile
                    {
                        MonthlyCost = 150m
                    }
                }
            },
            {
                ("Sql", "Basic"),
                new CloudResourceProfile
                {
                    ResourceType = "Sql",
                    Sku = "Basic",
                    Performance = new PerformanceProfile
                    {
                        BaseLatencyMs = 25,
                        TimeoutMs = 5000
                    },
                    Capacity = new CapacityProfile
                    {
                        RequestsPerSecond = 50
                    },
                    Cost = new CostProfile
                    {
                        MonthlyCost = 15m
                    }
                }
            }
        };
    }

    public CloudResourceProfile GetProfile(string resourceType, string sku)
    {
        if (!_profiles.TryGetValue((resourceType, sku), out var profile))
            throw new InvalidOperationException(
                $"No Azure profile found for type '{resourceType}' and sku '{sku}'.");

        return profile;
    }

    public IReadOnlyCollection<CloudResourceProfile> GetAllProfiles()
        => _profiles.Values;
}