using Microsoft.ML.Data;

namespace RealtimeDashboard.Api.Models;

public record SaleEvent(
    string Id,
    string Product,
    string Region,
    decimal Amount,
    int Quantity,
    DateTime Timestamp
);

public record DashboardSnapshot(
    decimal TotalRevenue,
    int TotalOrders,
    decimal AvgOrderValue,
    string TopProduct,
    string TopRegion,
    List<RegionStat> RegionStats,
    List<ProductStat> ProductStats,
    List<SaleEvent> RecentSales
);

public record RegionStat(string Region, decimal Revenue, int Orders);
public record ProductStat(string Product, decimal Revenue, int Units);

public record AnomalyAlert(
    string Id,
    string Product,
    string Region,
    decimal Amount,
    double Score,
    AnomalySeverity Severity,
    string Reason,
    DateTime DetectedAt
);

public enum AnomalySeverity { Warning, Critical }

public class SaleInputData
{
    public float Amount { get; set; }
}

public class SalePrediction
{
    [VectorType(3)]
    public double[] Prediction { get; set; } = new double[3];
}
