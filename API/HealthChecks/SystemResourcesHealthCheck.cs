using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace API.HealthChecks;

/// <summary>
/// Health Check personalizado que verifica los recursos del sistema
/// </summary>
public class SystemResourcesHealthCheck : IHealthCheck
{
    private readonly long _maxMemoryMegabytes;
    private readonly int _maxCpuPercentage;

    public SystemResourcesHealthCheck(long maxMemoryMegabytes = 500, int maxCpuPercentage = 90)
    {
        _maxMemoryMegabytes = maxMemoryMegabytes;
        _maxCpuPercentage = maxCpuPercentage;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var currentProcess = Process.GetCurrentProcess();
            
            // Memoria actual en MB
            var memoryUsedMB = currentProcess.WorkingSet64 / 1024 / 1024;
            var memoryPercentage = (memoryUsedMB * 100.0) / _maxMemoryMegabytes;

            // CPU (aproximado - en producción usar Performance Counters)
            var totalProcessorTime = currentProcess.TotalProcessorTime;
            var uptime = DateTime.UtcNow - currentProcess.StartTime.ToUniversalTime();
            var cpuUsagePercentage = (totalProcessorTime.TotalMilliseconds / uptime.TotalMilliseconds) * 100;

            var data = new Dictionary<string, object>
            {
                { "MemoryUsedMB", memoryUsedMB },
                { "MemoryLimitMB", _maxMemoryMegabytes },
                { "MemoryUsagePercentage", Math.Round(memoryPercentage, 2) },
                { "CpuUsagePercentage", Math.Round(cpuUsagePercentage, 2) },
                { "ThreadCount", currentProcess.Threads.Count },
                { "HandleCount", currentProcess.HandleCount }
            };

            // Determinar el estado
            if (memoryUsedMB > _maxMemoryMegabytes)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Memoria excedida: {memoryUsedMB}MB > {_maxMemoryMegabytes}MB",
                    data: data));
            }

            if (cpuUsagePercentage > _maxCpuPercentage)
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"CPU alta: {cpuUsagePercentage:F2}% > {_maxCpuPercentage}%",
                    data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                "Recursos del sistema dentro de límites normales",
                data: data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Error al verificar recursos del sistema",
                exception: ex));
        }
    }
}
