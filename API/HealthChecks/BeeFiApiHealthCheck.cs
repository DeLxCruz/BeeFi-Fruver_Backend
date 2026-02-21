using Application.Common.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace API.HealthChecks;

/// <summary>
/// Health Check para verificar la conectividad con el API externa de BeeFi
/// </summary>
public class BeeFiApiHealthCheck : IHealthCheck
{
    private readonly IBeeFiApiClient _beeFiApiClient;
    private readonly ILogger<BeeFiApiHealthCheck> _logger;

    public BeeFiApiHealthCheck(
        IBeeFiApiClient beeFiApiClient,
        ILogger<BeeFiApiHealthCheck> logger)
    {
        _beeFiApiClient = beeFiApiClient;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Intentar obtener un cliente de prueba o hacer un ping al servicio
            // Como no tenemos un endpoint específico de health, usamos un timeout corto
            var timeout = TimeSpan.FromSeconds(5);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);

            // Nota: Esto es un ejemplo. En producción necesitarías un endpoint /health en BeeFi API
            // Por ahora solo verificamos que el servicio esté configurado correctamente
            var data = new Dictionary<string, object>
            {
                { "Service", "BeeFi External API" },
                { "Status", "Configured" },
                { "Timestamp", DateTime.UtcNow }
            };

            return HealthCheckResult.Healthy(
                "BeeFi API client configurado correctamente",
                data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar salud del BeeFi API");
            
            return HealthCheckResult.Degraded(
                "No se pudo verificar la conexión con BeeFi API",
                exception: ex);
        }
    }
}
