using System.Text.Json.Serialization;
using RealtimeDashboard.Api.Hubs;
using RealtimeDashboard.Api.ML;
using RealtimeDashboard.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
        options.PayloadSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()));

builder.Services.AddSingleton<SalesDataService>();
builder.Services.AddSingleton<AnomalyDetectionService>();
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
app.MapGet("/health", () => Results.Ok(new
{
    status  = "healthy",
    runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription
}));

app.Run();
