using Api_Monitoring.Data;
using Api_Monitoring.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Api_Monitoring.Services
{
    public class LogProcessingJob : BackgroundService
    {
        private readonly LogBufferService _bufferService;
        private readonly ILogger<LogProcessingJob> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<MonitoringHub> _hubContext;

        public LogProcessingJob(
            LogBufferService bufferService,
            IServiceScopeFactory scopeFactory,
            ILogger<LogProcessingJob> logger,
            IHubContext<MonitoringHub> hubContext
            )
        {
            _bufferService = bufferService;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var logs = _bufferService.DequeueBatch(50);

                    if (logs.Any())
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<MonitoringDbContext>();
                            await dbContext.ApiLogs.AddRangeAsync(logs, stoppingToken);

                            await dbContext.SaveChangesAsync(stoppingToken);

                            _logger.LogInformation($"Saved {logs.Count()} logs to DB.");

                            await _hubContext.Clients.All.SendAsync("ReceiveLogs", logs, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing logs batch");
                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }
}
