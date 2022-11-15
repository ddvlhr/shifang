using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.BackgroundService;

public class PhysicalDataConvert : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IRepository<GroupRecord> _grRepo;
    private readonly ILogger<PhysicalDataConvert> _logger;
    private readonly Core.Models.Settings _settings;
    private Timer _timer;

    public PhysicalDataConvert(ILogger<PhysicalDataConvert> logger, IServiceProvider provider)
    {
        _logger = logger;
        _grRepo = provider.CreateScope().ServiceProvider.GetRequiredService<IRepository<GroupRecord>>();
        _settings = provider.CreateScope().ServiceProvider.GetRequiredService<IOptionsSnapshot<Core.Models.Settings>>()
            .Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Physical Data Convert Service running");

        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        var count = _grRepo.All().Count();
        var records = _grRepo.All().ToList();
        var specificationGroups = records.GroupBy(c => c.SpecificationId).ToList();


        _logger.LogInformation(
            $"weight id: {_settings.Weight}, options count: {count}, current time: {DateTime.Now:yyyy-MM-ddHH:mm:ss}");
    }
}