using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuYang.Core.Entities;
using FuYang.Infrastructure.DataBase;
using Microsoft.Extensions.DependencyInjection;

namespace FuYang.WorkService
{
    public class PhysicalDataConvert : BackgroundService
    {
        private readonly ILogger<PhysicalDataConvert> _logger;
        private readonly IRepository<GroupRecord> _grRepo;

        public PhysicalDataConvert(ILogger<PhysicalDataConvert> logger, IServiceProvider provider)
        {
            _logger = logger;
            _grRepo = provider.CreateScope().ServiceProvider.GetRequiredService<IRepository<GroupRecord>>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation("GroupRecord Table Count: {count}", _grRepo.All().Count());
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
