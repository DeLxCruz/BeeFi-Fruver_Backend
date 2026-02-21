namespace API.Middleware;

/// <summary>
/// Middleware para agregar headers de seguridad HTTP
/// Protege contra vulnerabilidades comunes (XSS, Clickjacking, etc.)
/// </summary>
public class SecurityHeadersMiddleware(RequestDelegate next, IWebHostEnvironment environment)
{
    private readonly RequestDelegate _next = next;
    private readonly IWebHostEnvironment _environment = environment;

    public async Task InvokeAsync(HttpContext context)
    {
        // ✅ Solo agregar headers si la respuesta NO ha empezado
        if (!context.Response.HasStarted)
        {
            // X-Content-Type-Options: Previene MIME sniffing
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // X-Frame-Options: Previene Clickjacking
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // X-XSS-Protection: Protección contra XSS (legacy browsers)
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Referrer-Policy: Controla información del referrer
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // Content-Security-Policy: Previene XSS y data injection
            // En desarrollo: permitir Browser Link y Hot Reload
            var csp = _environment.IsDevelopment()
                ? "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:; connect-src 'self' ws: http://localhost:* https://localhost:*"
                : "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:; connect-src 'self'";
            
            context.Response.Headers.Append("Content-Security-Policy", csp);

            // Permissions-Policy: Control de features del navegador
            context.Response.Headers.Append(
                "Permissions-Policy",
                "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

            // Strict-Transport-Security: Forzar HTTPS (solo en producción)
            if (context.Request.IsHttps)
            {
                context.Response.Headers.Append(
                    "Strict-Transport-Security",
                    "max-age=31536000; includeSubDomains");
            }
        }

        await _next(context);
    }
}