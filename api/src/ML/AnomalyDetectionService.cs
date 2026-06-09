using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using RealtimeDashboard.Api.Models;

namespace RealtimeDashboard.Api.ML;

public class AnomalyDetectionService
{
    private readonly MLContext _mlContext = new();
    private readonly Dictionary<string, List<float>> _windows = new();

    private const int MinSamples = 12;
    private const int PValueHistoryLength = 6;
    private const double Confidence = 90;

    public AnomalyAlert? Evaluate(SaleEvent sale)
    {
        var key = BuildKey(sale.Product, sale.Region);

        if (!_windows.TryGetValue(key, out var window))
        {
            window = new List<float>();
            _windows[key] = window;
        }

        window.Add((float)sale.Amount);

        if (window.Count < MinSamples)
            return null;

        var isAnomaly = DetectSpike(window, out var score, out var pValue);

        if (!isAnomaly)
            return null;

        var severity  = pValue < 0.01 ? AnomalySeverity.Critical : AnomalySeverity.Warning;
        var avg       = (decimal)window.TakeLast(PValueHistoryLength).Average();
        var direction = sale.Amount > avg ? "spike" : "drop";
        var pct       = Math.Abs(((double)sale.Amount - (double)avg) / (double)avg * 100);
        var reason    = $"Unusual {direction} of {pct:F0}% vs recent {sale.Product} sales in {sale.Region} (avg ${avg:F0})";

        return new AnomalyAlert(
            Guid.NewGuid().ToString("N")[..8],
            sale.Product,
            sale.Region,
            sale.Amount,
            Math.Round(score, 4),
            severity,
            reason,
            DateTime.UtcNow
        );
    }

    private bool DetectSpike(List<float> window, out double score, out double pValue)
    {
        score  = 0;
        pValue = 1;

        var data     = window.Select(a => new SaleInputData { Amount = a }).ToList();
        var dataView = _mlContext.Data.LoadFromEnumerable(data);

        var pipeline = _mlContext.Transforms.DetectIidSpike(
            outputColumnName:    nameof(SalePrediction.Prediction),
            inputColumnName:     nameof(SaleInputData.Amount),
            confidence:          Confidence,
            pvalueHistoryLength: PValueHistoryLength
        );

        var transformed = pipeline.Fit(dataView).Transform(dataView);

        var last = _mlContext.Data
            .CreateEnumerable<SalePrediction>(transformed, reuseRowObject: false)
            .Last();

        score  = last.Prediction[1];
        pValue = last.Prediction[2];

        return last.Prediction[0] == 1;
    }

    private static string BuildKey(string product, string region)
        => $"{product}::{region}".ToLowerInvariant();

    public int  WindowCount  => _windows.Count;
    public int  TotalSamples => _windows.Values.Sum(w => w.Count);
    public bool IsReady      => _windows.Values.Any(w => w.Count >= MinSamples);
}
