using System.Diagnostics;

namespace API.Middleware;

/// <summary>
/// Middleware para monitoreo de performance
/// Mide tiempos de respuesta y alerta sobre requests lentos
/// </summary>
public class PerformanceMonitoringMiddleware(
    RequestDelegate next,
    ILogger<PerformanceMonitoringMiddleware> logger,
    IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger = logger;
    private readonly int _slowRequestThresholdMs = configuration.GetValue<int>("Performance:SlowRequestThresholdMs", 1000);

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // Alertar sobre requests lentos
            if (elapsedMs > _slowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "⚠️ SLOW REQUEST: {Method} {Path} took {ElapsedMs}ms (threshold: {ThresholdMs}ms) - Status: {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMs,
                    _slowRequestThresholdMs,
                    context.Response.StatusCode);
            }

            // ✅ Solo agregar header si la respuesta NO ha empezado
            if (!context.Response.HasStarted)
            {
                context.Response.Headers.Append("X-Response-Time-Ms", elapsedMs.ToString());
            }
        }
    }
}