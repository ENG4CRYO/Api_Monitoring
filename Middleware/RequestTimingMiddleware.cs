using Api_Monitoring.Core;
using Api_Monitoring.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Api_Monitoring_Middleware.Middleware
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LogBufferService _bufferService;

        public RequestTimingMiddleware(RequestDelegate next, LogBufferService bufferService)
        {
            _next = next;
            _bufferService = bufferService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && (
                path.Contains("api-watchdog") ||
                path.Contains("monitoringhub") ||
                path.Contains("monitoring-dashboard") ||
                path.Contains("browserlink")))
            {
               
                await _next(context);
                return;
            }
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            await _next(context);

            stopwatch.Stop();

            var log = new ApiLog
            {
                Timestamp = DateTime.UtcNow,
                Method = context.Request.Method,
                Path = context.Request.Path,
                StatusCode = context.Response.StatusCode,
                DurationMs = stopwatch.ElapsedMilliseconds,
                QueryString = context.Request.QueryString.ToString(),
                IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
            };

            _bufferService.AddLog(log);
        }

    }
}
