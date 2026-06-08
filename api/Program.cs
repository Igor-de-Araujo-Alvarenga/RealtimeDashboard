using RealtimeDashboard.Api.Hubs;
using RealtimeDashboard.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<SalesDataService>();
builder.Services.AddHostedService<SalesBroadcastService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

app.UseCors();
app.MapHub<DashboardHub>("/hubs/dashboard");
app.MapGet("/health", () => Results.Ok(new { status = "healthy", runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription }));

app.Run();
