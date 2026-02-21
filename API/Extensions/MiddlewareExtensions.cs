using API.Middleware;

namespace API.Extensions;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Registra todos los middlewares personalizados en el orden correcto
    /// </summary>
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<GlobalExceptionHandlingMiddleware>()
            .UseMiddleware<SecurityHeadersMiddleware>()
            .UseMiddleware<RequestLoggingMiddleware>()
            .UseMiddleware<PerformanceMonitoringMiddleware>();
    }
}