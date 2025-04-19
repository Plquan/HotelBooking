using Hotel.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public class ScheduleService : BackgroundService
    {
        private readonly ILogger<ScheduleService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ScheduleService(ILogger<ScheduleService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Service is starting.");
            var task1 = Task.Run(() => CheckValidToken(stoppingToken));
            var task2 = Task.Run(() => CheckRoomStatus(stoppingToken));
            await Task.WhenAll(task1, task2);
            _logger.LogInformation("Background Service is stopping.");
        }
        private async Task CheckValidToken(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Task 1 running at: {DateTime.Now}");
                using (var scope = _serviceProvider.CreateScope())  
                {
                    var context = scope.ServiceProvider.GetRequiredService<HotelContext>();
                    var expiredTokens = await context.InvalidatedTokens
                        .Where(t => t.expiryTime <= DateTime.Now)
                        .ToListAsync();
                    context.InvalidatedTokens.RemoveRange(expiredTokens);
                    await context.SaveChangesAsync();
                }
                await Task.Delay(3600000, stoppingToken);
            }
        }

        private async Task CheckRoomStatus(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Task 2 running at: {DateTime.Now}");

                await Task.Delay(3600000, stoppingToken); 
            }
        }
    }
  }
