using ArchSim.Cloud.Models;

namespace ArchSim.Cloud.Abstractions;

public interface ICloudCatalog
{
    CloudResourceProfile GetProfile(string resourceType, string sku);
    IReadOnlyCollection<CloudResourceProfile> GetAllProfiles();
}
