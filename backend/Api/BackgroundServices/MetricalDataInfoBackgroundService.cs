using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Services.MetricalData;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Api.BackgroundServices;

public class MetricalDataInfoBackgroundService : BackgroundService
{
    private readonly ILogger<MetricalDataInfoBackgroundService> _logger;
    private readonly IMetricalDataService _metricalDataService;
    public MetricalDataInfoBackgroundService(ILogger<MetricalDataInfoBackgroundService> logger,
    IMetricalDataService metricalDataService)
    {
        _logger = logger;
        _metricalDataService = metricalDataService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("metricalData info service hosted service is running");
        await doWork(stoppingToken);
    }

    private async Task doWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("metricalData info service hosted service is working");
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("metricalData info service hosted service is stopping");
        return base.StopAsync(cancellationToken);
    }
}