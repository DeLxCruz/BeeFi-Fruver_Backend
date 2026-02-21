using API.HealthChecks;
using Asp.Versioning;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;

namespace API;

// Clase para mapear configuración de Health Checks desde appsettings
internal class HealthCheckEndpoint
{
    public string Name { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
}

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Controllers
        services.AddControllers();

        // Problem Details (RFC 9457)
        services.AddProblemDetails();

        // API Versioning (Milan Jovanovic pattern)
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        // Swagger/OpenAPI
        services.AddSwaggerConfiguration();

        // CORS
        services.AddCorsConfiguration(configuration);

        // Health Checks
        services.AddHealthChecksConfiguration(configuration);

        // Rate Limiting
        services.AddRateLimiterConfiguration();

        // 🔧 Deshabilitar Response Compression para Health Checks UI (fix chunked encoding)
        services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 104857600; // 100 MB
        });

        return services;
    }

    private static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BeeFi API",
                Version = "v1",
                Description = "API para plataforma de fruvers con integración BeeFi",
                Contact = new OpenApiContact
                {
                    Name = "BeeFi Team",
                    Email = "soporte@beefi.com",
                    Url = new Uri("https://beefi.com")
                },
                License = new OpenApiLicense
                {
                    Name = "Uso privado",
                }
            });

            // 🔐 Configurar JWT en Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header usando el esquema Bearer. 
                              Ingresa 'Bearer' [espacio] y luego tu token.
                              Ejemplo: 'Bearer eyJhbGc...'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            // Incluir comentarios XML si existen
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    private static IServiceCollection AddRateLimiterConfiguration(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Política global: 100 requests por minuto por IP
            options.AddFixedWindowLimiter("GlobalPolicy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 100;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 10;
            });

            // Política estricta para Auth: 10 intentos por minuto por IP
            options.AddFixedWindowLimiter("AuthPolicy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 10;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0;
            });

            // Política para endpoints públicos: 200 por minuto
            options.AddFixedWindowLimiter("PublicPolicy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 200;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 5;
            });
        });

        return services;
    }

    private static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? new[] { "http://localhost:3000" };

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFlutter", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });

            // Política para desarrollo (más permisiva)
            options.AddPolicy("AllowDevelopment", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }

    private static IServiceCollection AddHealthChecksConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHealthChecks()
            // Check de la base de datos SQL Server
            .AddSqlServer(
                connectionString: configuration.GetConnectionString("DefaultConnection")!,
                healthQuery: "SELECT 1;",
                name: "sql-server",
                failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                tags: new[] { "db", "sql", "sqlserver" })
            // Check de DbContext
            .AddDbContextCheck<Infrastructure.Persistence.ApplicationDbContext>(
                name: "ef-core-dbcontext",
                failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                tags: new[] { "db", "ef-core" })
            // Check personalizado de recursos del sistema
            .AddCheck<SystemResourcesHealthCheck>(
                name: "system-resources",
                failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
                tags: new[] { "system", "resources" })
            // Check personalizado de API externa BeeFi
            .AddCheck<BeeFiApiHealthCheck>(
                name: "beefi-api",
                failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
                tags: new[] { "external", "beefi" });

        // Health Checks UI Dashboard
        services
            .AddHealthChecksUI(setup =>
            {
                // Leer configuración desde appsettings.json
                var healthChecksConfig = configuration.GetSection("HealthChecksUI:HealthChecks").Get<List<HealthCheckEndpoint>>();
                
                if (healthChecksConfig != null)
                {
                    foreach (var endpoint in healthChecksConfig)
                    {
                        setup.AddHealthCheckEndpoint(endpoint.Name, endpoint.Uri);
                    }
                }

                setup.SetEvaluationTimeInSeconds(configuration.GetValue<int>("HealthChecksUI:EvaluationTimeInSeconds", 10));
                setup.SetMinimumSecondsBetweenFailureNotifications(configuration.GetValue<int>("HealthChecksUI:MinimumSecondsBetweenFailureNotifications", 60));
                setup.MaximumHistoryEntriesPerEndpoint(50);
            })
            .AddInMemoryStorage();

        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        // Health Checks Endpoints
        app.MapHealthChecksEndpoints();

        return app;
    }

    private static void MapHealthChecksEndpoints(this WebApplication app)
    {
        // ✅ Endpoint routing moderno .NET 8: usar app.Map* directamente (sin UseEndpoints)
        
        // Endpoint básico
        app.MapHealthChecks("/health");

        // Endpoint detallado con información completa en formato JSON
        app.MapHealthChecks("/health/details", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Endpoint para verificar solo la base de datos
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("db"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Endpoint para verificar que la API está viva
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });

        // ✅ Health Checks UI Dashboard: rutas absolutas para evitar ERR_INCOMPLETE_CHUNKED_ENCODING
        app.MapHealthChecksUI(config =>
        {
            config.UIPath = "/health-ui";              // Página del dashboard
            config.ApiPath = "/health-ui-api";         // API que consume el UI
            config.ResourcesPath = "/ui/resources";    // Recursos estáticos (CSS, JS)
            config.UseRelativeApiPath = false;         // Fuerza rutas absolutas
            config.UseRelativeResourcesPath = false;   // Fuerza rutas absolutas
        });
    }
}