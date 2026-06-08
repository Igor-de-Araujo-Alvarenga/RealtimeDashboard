using Microsoft.AspNetCore.SignalR;

namespace RealtimeDashboard.Api.Hubs;

public class DashboardHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
}
