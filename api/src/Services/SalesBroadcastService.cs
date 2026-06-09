using Microsoft.AspNetCore.SignalR;
using RealtimeDashboard.Api.Hubs;
using RealtimeDashboard.Api.ML;

namespace RealtimeDashboard.Api.Services;

public class SalesBroadcastService(
    IHubContext<DashboardHub> hub,
    SalesDataService salesData,
    AnomalyDetectionService anomalyDetection) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var sale     = salesData.GenerateSale();
            var snapshot = salesData.GetSnapshot();

            await hub.Clients.All.SendAsync("SaleOccurred",     sale,     stoppingToken);
            await hub.Clients.All.SendAsync("SnapshotUpdated",  snapshot, stoppingToken);

            var alert = anomalyDetection.Evaluate(sale);
            if (alert is not null)
                await hub.Clients.All.SendAsync("AnomalyDetected", alert, stoppingToken);

            var delay = anomalyDetection.IsReady
                ? Random.Shared.Next(400, 900)
                : Random.Shared.Next(80, 150);

            await Task.Delay(delay, stoppingToken);
        }
    }
}
