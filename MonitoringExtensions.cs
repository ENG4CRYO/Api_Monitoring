using Api_Monitoring.Data;
using Api_Monitoring.Hubs;
using Api_Monitoring.Services;
using Api_Monitoring_Middleware.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;



public static class MonitoringExtensions
{
    
    public static IServiceCollection ApiWatchdog(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<MonitoringDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddSingleton<LogBufferService>();
        services.AddHostedService<LogProcessingJob>();

     
        services.AddSignalR();

        return services;
    }

 
    public static IApplicationBuilder UseApiWatchdog(this IApplicationBuilder app)
    {
   
        app.UseMiddleware<RequestTimingMiddleware>();


        
        if (app is WebApplication webApp)
        {
            webApp.MapHub<MonitoringHub>("/monitoringHub");

            async Task ServeEmbeddedFile(HttpContext context, string fileName, string contentType)
            {
                var assembly = Assembly.GetExecutingAssembly();
      
                var resourceName = $"ApiWatchdog.UI.{fileName}";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                context.Response.ContentType = contentType;
                using var reader = new StreamReader(stream);
                await context.Response.WriteAsync(await reader.ReadToEndAsync());
            }


            webApp.MapGet("/monitoring-dashboard", async context =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "ApiWatchdog.UI.dashboard.html";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    await context.Response.WriteAsync("Dashboard file not found!");
                    return;
                }

                using var reader = new StreamReader(stream);
                var html = await reader.ReadToEndAsync();

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(html);
            });

            webApp.MapGet("/api-watchdog/stats", async (MonitoringDbContext db) =>
            {
                var now = DateTime.UtcNow;
                var oneMinuteAgo = now.AddMinutes(-1);
                var twentyFourHoursAgo = now.AddHours(-24);

             
                var currentRpm = await db.ApiLogs.CountAsync(l => l.Timestamp >= oneMinuteAgo);

                var dailyStats = await db.ApiLogs
                    .Where(l => l.Timestamp >= twentyFourHoursAgo)
                    .GroupBy(l => 1) 
                    .Select(g => new
                    {
                        Total = g.Count(),
                        Failed = g.Count(l => l.StatusCode >= 500),
                       
                        AvgLatency = g.Average(l => l.DurationMs)
                    })
                    .FirstOrDefaultAsync();

             
                var rawTraffic = await db.ApiLogs
                    .Where(l => l.Timestamp >= twentyFourHoursAgo)
                    .Select(l => l.Timestamp) 
                    .ToListAsync();

                var trafficChart = rawTraffic
                    .GroupBy(t => t.Hour)
                    .Select(g => new { Hour = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Hour)
                    .ToList();

                var statusChart = await db.ApiLogs
                    .Where(l => l.Timestamp >= twentyFourHoursAgo)
                    .GroupBy(l => l.StatusCode)
                    .Select(g => new { Code = g.Key, Count = g.Count() })
                    .ToListAsync();

                return Results.Ok(new
                {
                    rpm = currentRpm,
                    totalRequests = dailyStats?.Total ?? 0,
                    failedRequests = dailyStats?.Failed ?? 0,
                    avgDuration = Math.Round(dailyStats?.AvgLatency ?? 0, 1),
                    traffic = trafficChart,
                    statusDistribution = statusChart
                });
            });
        }

        using (var scope = app.ApplicationServices.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MonitoringDbContext>();

            db.Database.Migrate();
        }




        return app;
    }
}