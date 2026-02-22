using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Services;
using Application.Common.Interfaces;
using Infrastructure.Authentication;
using Infrastructure.Persistence;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ========================================
        // 🗄️ DATABASE
        // ========================================
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        // ========================================
        // 🔐 AUTHENTICATION & JWT
        // ========================================
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(jwtSettings));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // En producción cambiar a true
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No tolerancia al tiempo expirado
            };

            // Eventos para debugging (opcional)
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers["Token-Expired"] = "true";
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    // Log cuando falla autenticación
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    // Token validado exitosamente
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        // ========================================
        // 📧 EMAIL SERVICE
        // ========================================
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.AddScoped<IEmailService, EmailService>();

        // ========================================
        // 🕒 DATE TIME PROVIDER
        // ========================================
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // ========================================
        // 👤 CURRENT USER SERVICE
        // ========================================
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // ========================================
        // 💳 PAYMENT GATEWAY
        // ========================================
        services.AddScoped<IPaymentGateway, MockPaymentGateway>();

        // ========================================
        // 🔔 PUSH NOTIFICATION SERVICE
        // ========================================
        services.AddScoped<IPushNotificationService, MockPushNotificationService>();

        // ========================================
        // 💾 CACHING
        // ========================================
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();

        // ========================================
        // 📊 PRICE REFERENCE SERVICE
        // ========================================
        services.AddScoped<IPriceReferenceService, PriceReferenceService>();

        // ========================================
        // 🌐 BEEFI API CLIENT
        // ========================================
        services.Configure<BeeFiApiSettings>(configuration.GetSection(BeeFiApiSettings.SectionName));
        services.AddHttpClient<IBeeFiApiClient, BeeFiApiClient>();

        return services;
    }
}
