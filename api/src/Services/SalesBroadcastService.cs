using Microsoft.AspNetCore.SignalR;
using RealtimeDashboard.Api.Hubs;
using RealtimeDashboard.Api.Services;

namespace RealtimeDashboard.Api.Services;

public class SalesBroadcastService(
    IHubContext<DashboardHub> hub,
    SalesDataService salesData) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var sale = salesData.GenerateSale();
            var snapshot = salesData.GetSnapshot();

            await hub.Clients.All.SendAsync("SaleOccurred", sale, stoppingToken);
            await hub.Clients.All.SendAsync("SnapshotUpdated", snapshot, stoppingToken);

            var delay = Random.Shared.Next(600, 2000);
            await Task.Delay(delay, stoppingToken);
        }
    }
}
