using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Extensions;
using Infrastructure.Helper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Api.BackgroundServices;

public class ServerInfoBackgroundService: BackgroundService
{
    private readonly ILogger<ServerInfoBackgroundService> _logger;

    public ServerInfoBackgroundService(
        ILogger<ServerInfoBackgroundService> logger)
    {
        _logger = logger;
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
            // var onlineUserTotal = ServerHub.OnlineUsers.Count;
            // var result = new Response(0, "", onlineUserTotal);
            // await _hubContext.Clients.All.SendAsync("OnlineUserMessage", JsonConvert.SerializeObject(result), cancellationToken: stoppingToken);
            _logger.LogInformation($"系统内存信息: {SystemInfo.GetMemoryInfo()}");
            await Task.Delay(5000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("server info service hosted service is stopping");
        await base.StopAsync(cancellationToken);
    }
}