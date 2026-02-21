using API;
using API.Extensions;
using Infrastructure;
using Application;
using Serilog;

// ========================================
// 🔧 CONFIGURAR SERILOG desde appsettings.json
// ========================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
    Log.Information("🚀 Starting BeeFi Fruver API");

    var builder = WebApplication.CreateBuilder(args);

    // ========================================
    // 🔧 CONFIGURAR KESTREL (Fix ERR_INCOMPLETE_CHUNKED_ENCODING)
    // ========================================
    if (builder.Environment.IsDevelopment())
    {
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ConfigureEndpointDefaults(listenOptions =>
            {
                // Forzar HTTP/1.1 para evitar problemas con HTTP/2
                listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
            });
            
            // Aumentar límites para archivos grandes
            serverOptions.Limits.MaxRequestBodySize = 104857600; // 100 MB
            serverOptions.Limits.MinResponseDataRate = null; // Sin límite de tasa de respuesta
            serverOptions.Limits.MinRequestBodyDataRate = null; // Sin límite de tasa de request
        });
    }

    // ========================================
    // �📦 REGISTRAR SERVICIOS
    // ========================================

    // Usar Serilog
    builder.Host.UseSerilog();

    // Agregar capas de la aplicación
    builder.Services
        .AddPresentation(builder.Configuration)      // API Layer
        .AddApplication()                            // Application Layer
        .AddInfrastructure(builder.Configuration);   // Infrastructure Layer

    var app = builder.Build();

    // ========================================
    // 🔧 CONFIGURAR MIDDLEWARE PIPELINE
    // ========================================

    // Swagger (solo en desarrollo)
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "BeeFi Fruver API V1");
            c.RoutePrefix = string.Empty; // Swagger en la raíz
            c.DisplayRequestDuration();
        });
    }

    // ⚠️ ORDEN CRÍTICO DE MIDDLEWARES

    // Serilog Request Logging (captura automática de requests)
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        options.GetLevel = (httpContext, elapsed, ex) => elapsed > 1000
            ? Serilog.Events.LogEventLevel.Warning
            : Serilog.Events.LogEventLevel.Information;
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
        };
    });

    // Middlewares personalizados
    app.UseCustomMiddlewares();

    // HTTPS Redirection
    app.UseHttpsRedirection();

    // Static Files (necesario para Health Checks UI)
    app.UseStaticFiles();

    // CORS
    app.UseCors(app.Environment.IsDevelopment() ? "AllowDevelopment" : "AllowFlutter");

    // Rate Limiting
    app.UseRateLimiter();

    // Routing (DEBE ir antes de Authentication/Authorization)
    app.UseRouting();

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Endpoints (Controllers + Health Checks)
    app.MapControllers();
    app.UsePresentation(); // Health Checks endpoints

    Log.Information("✅ BeeFi Fruver API started successfully on {Environment}", app.Environment.EnvironmentName);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}