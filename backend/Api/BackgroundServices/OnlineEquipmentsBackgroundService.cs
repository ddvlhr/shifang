using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Hubs;
using Infrastructure.Response;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.BackgroundServices;

public class OnlineEquipmentsBackgroundService: BackgroundService
{
    private readonly ILogger<OnlineEquipmentsBackgroundService> _logger;
    private readonly IHubContext<ServerHub> _hubContext;

    public OnlineEquipmentsBackgroundService(
        ILogger<OnlineEquipmentsBackgroundService> logger,
        IHubContext<ServerHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("equipments service hosted service is running");

        await doWork(stoppingToken);
    }
    
    private async Task doWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("equipments service hosted service is working");
        while (!stoppingToken.IsCancellationRequested)
        {
            var equipments = ServerHub.Equipments;
            var result = new Response(0, "", equipments);
            await _hubContext.Clients.All.SendAsync("EquipmentsMessage", JsonConvert.SerializeObject(result), cancellationToken: stoppingToken);
            await Task.Delay(30000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("equipments service hosted service is stopping");
        await base.StopAsync(cancellationToken);
    }
}