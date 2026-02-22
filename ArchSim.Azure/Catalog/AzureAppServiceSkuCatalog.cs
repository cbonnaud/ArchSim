namespace ArchSim.Azure.Catalog;

public static class AzureAppServiceSkuCatalog
{
    private static readonly Dictionary<string, (double capacity, decimal cost)> _skus =
        new()
        {
            { "P1V2", (500, 200) },
            { "P2V2", (1000, 400) },
            { "B1", (100, 50) }
        };

    public static (double capacity, decimal cost) Resolve(string sku)
    {
        if (!_skus.TryGetValue(sku, out var value))
            throw new ArgumentException($"Unknown App Service SKU '{sku}'.");

        return value;
    }
}