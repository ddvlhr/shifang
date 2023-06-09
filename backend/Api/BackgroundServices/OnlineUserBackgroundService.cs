using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Hubs;
using Infrastructure.Response;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlSugar;

namespace Api.BackgroundServices;

public class OnlineUserBackgroundService: BackgroundService
{
    private readonly ILogger<OnlineUserBackgroundService> _logger;
    private readonly IHubContext<ServerHub> _hubContext;

    public OnlineUserBackgroundService(
        ILogger<OnlineUserBackgroundService> logger,
        IHubContext<ServerHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("online user service hosted service is running");

        await doWork(stoppingToken);
    }
    
    private async Task doWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("online user service hosted service is working");
        while (!stoppingToken.IsCancellationRequested)
        {
            var onlineUserTotal = ServerHub.OnlineUsers.Count(c => c.Machine > 0);
            var result = new Response(0, "", onlineUserTotal);
            await _hubContext.Clients.All.SendAsync("OnlineUserMessage", JsonConvert.SerializeObject(result), cancellationToken: stoppingToken);
            await Task.Delay(60000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("online user service hosted service is stopping");
        await base.StopAsync(cancellationToken);
    }
}