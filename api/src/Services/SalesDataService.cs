using RealtimeDashboard.Api.Models;

namespace RealtimeDashboard.Api.Services;

public class SalesDataService
{
    private readonly List<SaleEvent> _sales = new();
    private readonly string[] _products = ["Laptop Pro", "Wireless Mouse", "Mechanical Keyboard", "4K Monitor", "USB-C Hub", "Webcam HD", "Standing Desk", "Chair Ergo"];
    private readonly string[] _regions = ["North America", "Europe", "Asia Pacific", "Latin America", "Middle East"];
    private readonly Random _rng = new();

    public SaleEvent GenerateSale()
    {
        var product = _products[_rng.Next(_products.Length)];
        var region = _regions[_rng.Next(_regions.Length)];
        var quantity = _rng.Next(1, 6);
        var unitPrice = product switch
        {
            "Laptop Pro" => _rng.Next(900, 1800),
            "4K Monitor" => _rng.Next(300, 700),
            "Standing Desk" => _rng.Next(400, 900),
            "Chair Ergo" => _rng.Next(250, 600),
            _ => _rng.Next(30, 200)
        };

        var sale = new SaleEvent(
            Guid.NewGuid().ToString("N")[..8],
            product,
            region,
            unitPrice * quantity,
            quantity,
            DateTime.UtcNow
        );

        _sales.Add(sale);
        if (_sales.Count > 500) _sales.RemoveAt(0);

        return sale;
    }

    public DashboardSnapshot GetSnapshot()
    {
        if (_sales.Count == 0)
            return new DashboardSnapshot(0, 0, 0, "-", "-", [], [], []);

        var totalRevenue = _sales.Sum(s => s.Amount);
        var totalOrders = _sales.Count;
        var avgOrderValue = totalRevenue / totalOrders;

        var regionStats = _sales
            .GroupBy(s => s.Region)
            .Select(g => new RegionStat(g.Key, g.Sum(s => s.Amount), g.Count()))
            .OrderByDescending(r => r.Revenue)
            .ToList();

        var productStats = _sales
            .GroupBy(s => s.Product)
            .Select(g => new ProductStat(g.Key, g.Sum(s => s.Amount), g.Sum(s => s.Quantity)))
            .OrderByDescending(p => p.Revenue)
            .ToList();

        return new DashboardSnapshot(
            Math.Round(totalRevenue, 2),
            totalOrders,
            Math.Round(avgOrderValue, 2),
            productStats.First().Product,
            regionStats.First().Region,
            regionStats,
            productStats,
            _sales.TakeLast(10).Reverse().ToList()
        );
    }
}
