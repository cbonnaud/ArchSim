namespace ArchSim.Azure.Catalog;

public static class AzureSqlSkuCatalog
{
    private static readonly Dictionary<string, (double capacity, decimal cost)> _skus =
        new()
        {
            { "Basic", (200, 100) },
            { "GeneralPurpose", (500, 300) }
        };

    public static (double capacity, decimal cost) Resolve(string sku)
    {
        if (!_skus.TryGetValue(sku, out var value))
            throw new ArgumentException($"Unknown SQL SKU '{sku}'.");

        return value;
    }
}