using ArchSim.Azure.Catalog;

namespace ArchSim.Azure.Tests.Catalog;

public class AzureCatalogTests
{
    private readonly AzureCatalog _catalog = new();

    [Fact]
    public void GetProfile_ShouldReturnCorrectProfile_WhenSkuExists()
    {
        // Act
        var profile = _catalog.GetProfile("AppService", "B1");

        // Assert
        Assert.NotNull(profile);
        Assert.Equal("AppService", profile.ResourceType);
        Assert.Equal("B1", profile.Sku);

        Assert.Equal(20, profile.Performance.BaseLatencyMs);
        Assert.Equal(1000, profile.Performance.TimeoutMs);

        Assert.Equal(100, profile.Capacity.RequestsPerSecond);

        Assert.Equal(55m, profile.Cost.MonthlyCost);
    }

    [Fact]
    public void GetProfile_ShouldThrowKeyNotFoundException_WhenSkuDoesNotExist()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _catalog.GetProfile("AppService", "InvalidSku"));
    }

    [Fact]
    public void Profiles_ShouldHaveValidBusinessValues()
    {
        var profile = _catalog.GetProfile("AppService", "B1");

        Assert.True(profile.Performance.BaseLatencyMs >= 0);
        Assert.True(profile.Performance.TimeoutMs > 0);
        Assert.True(profile.Capacity.RequestsPerSecond > 0);
        Assert.True(profile.Cost.MonthlyCost >= 0);
    }
}