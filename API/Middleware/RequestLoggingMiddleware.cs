using System.Diagnostics;

namespace API.Middleware;

/// <summary>
/// Middleware para logging detallado de requests
/// Complementa el logging automático de Serilog con información adicional
/// </summary>
public class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;

        // Log request básico
        _logger.LogInformation(
            "Request {RequestId}: {Method} {Path} started",
            requestId,
            context.Request.Method,
            context.Request.Path);

        try
        {
            await _next(context);

            stopwatch.Stop();

            // Log response exitoso
            _logger.LogInformation(
                "Request {RequestId}: {Method} {Path} completed with {StatusCode} in {ElapsedMs}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log error (será capturado por GlobalExceptionHandlingMiddleware)
            _logger.LogError(
                ex,
                "Request {RequestId}: {Method} {Path} failed after {ElapsedMs}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}