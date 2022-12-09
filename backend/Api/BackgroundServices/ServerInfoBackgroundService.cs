using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Helper;
using Infrastructure.Response;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Api.Hubs;
using Newtonsoft.Json;

namespace Api.BackgroundServices;

public class ServerInfoBackgroundService : BackgroundService
{
    private readonly ILogger<ServerInfoBackgroundService> _logger;
    private readonly IHubContext<ServerHub> _hubContext;

    public ServerInfoBackgroundService(
        ILogger<ServerInfoBackgroundService> logger,
        IHubContext<ServerHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("server info service hosted service is running");

        await doWork(stoppingToken);
    }

    private async Task doWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("online user service hosted service is working");
        while (!stoppingToken.IsCancellationRequested)
        {
            var systemInfo = ComputerHelper.GetComputerInfo();
            var response = new Response(0, "success", systemInfo);
            await _hubContext.Clients.All.SendAsync("ServerInfoMessage", JsonConvert.SerializeObject(response), cancellationToken: stoppingToken);
            await Task.Delay(5000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("server info service hosted service is stopping");
        await base.StopAsync(cancellationToken);
    }
}