# ✅ Checklist - Configuración DTOs y Manejo de Errores

## 📦 Estructura Creada

- [x] `API/Contracts/Common/ErrorResponse.cs` - Response de error estandarizado
- [x] `API/Contracts/Authentication/RegisterRequest.cs` - Request para registro
- [x] `API/Contracts/Authentication/LoginRequest.cs` - Request para login
- [x] `API/Contracts/Authentication/RefreshTokenRequest.cs` - Request para refresh token
- [x] `API/Contracts/Authentication/LogoutRequest.cs` - Request para logout
- [x] `API/Contracts/Authentication/UserProfileResponse.cs` - Response de perfil
- [x] `API/Middleware/GlobalExceptionHandlingMiddleware.cs` - Middleware mejorado
- [x] `API/Controllers/AuthenticationController.cs` - Actualizado con nuevos contratos

## 🔧 Pendiente de Verificar

### 1. Middleware Registrado
- [x] Verificar que `GlobalExceptionHandlingMiddleware` esté registrado en `Program.cs`
- [x] Verificar que esté en el orden correcto (primero en el pipeline)

### 2. Referencias a los Contratos
Actualizar las siguientes clases para usar los nuevos contratos:

```csharp
// En Controllers
using API.Contracts.Authentication;
using API.Contracts.Common;
```

### 3. FluentValidation
Verificar que el ValidationBehavior esté configurado:

- [ ] Revisar `Application/DependencyInjection.cs`
- [ ] Debe tener: `services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());`
- [ ] Debe registrar: `ValidationPipelineBehavior`

### 4. Swagger Documentation
Verificar que Swagger esté configurado para mostrar los DTOs:

- [ ] Abrir `https://localhost:7001/swagger`
- [ ] Verificar que `ErrorResponse` aparezca en Schemas
- [ ] Verificar que cada endpoint muestre los DTOs correctos

### 5. Testing
Probar cada endpoint con diferentes escenarios:

#### Register Endpoint
- [ ] Registro exitoso → 201 Created + RegisterResponse
- [ ] Email duplicado → 400 + ErrorResponse (User.EmailExists)
- [ ] Validación fallida → 400 + ErrorResponse con validationErrors
- [ ] Error de servidor → 500 + ErrorResponse

#### Login Endpoint
- [ ] Login exitoso → 200 + LoginResponse con tokens
- [ ] Credenciales incorrectas → 401 + ErrorResponse
- [ ] Usuario inactivo → 401 + ErrorResponse
- [ ] Validación fallida → 400 + ErrorResponse con validationErrors

#### RefreshToken Endpoint
- [ ] Token válido → 200 + RefreshTokenResponse con nuevos tokens
- [ ] Token expirado → 401 + ErrorResponse (RefreshToken.Expired)
- [ ] Token revocado → 401 + ErrorResponse (RefreshToken.Revoked)
- [ ] Token inválido → 401 + ErrorResponse (RefreshToken.Invalid)

#### Logout Endpoint
- [ ] Logout dispositivo actual → 200 + LogoutResponse
- [ ] Logout todos los dispositivos → 200 + LogoutResponse
- [ ] Sin token → 400 + ErrorResponse
- [ ] Usuario no encontrado → 400 + ErrorResponse

### 6. Logging
Verificar que los logs se estén generando correctamente:

- [ ] Revisar archivo de log: `API/logs/log-{fecha}.txt`
- [ ] Verificar que incluya TraceId
- [ ] Verificar que incluya detalles de la excepción
- [ ] Verificar niveles de log (Information, Warning, Error)

### 7. Environment-Specific Behavior
Verificar comportamiento según el ambiente:

#### Development
- [ ] ErrorResponse incluye `Details` con StackTrace
- [ ] Swagger está disponible en `/`
- [ ] Logs más verbosos

#### Production
- [ ] ErrorResponse NO incluye `Details`
- [ ] Swagger está deshabilitado
- [ ] Mensajes de error genéricos

## 🚀 Próximas Mejoras Recomendadas

### 1. Crear DTOs para Otros Features
```
API/Contracts/
├── Products/
│   ├── CreateProductRequest.cs
│   ├── UpdateProductRequest.cs
│   ├── ProductResponse.cs
│   └── ProductListResponse.cs
├── Orders/
│   ├── CreateOrderRequest.cs
│   ├── OrderResponse.cs
│   └── OrderStatusResponse.cs
└── Users/
    ├── UpdateUserRequest.cs
    └── UserResponse.cs
```

### 2. Implementar GetUserProfile Query
```csharp
// Application/Features/Authentication/GetProfile/GetProfileQuery.cs
public record GetProfileQuery(Guid UserId) 
    : IRequest<Result<UserProfileResponse>>;

// Application/Features/Authentication/GetProfile/GetProfileQueryHandler.cs
public class GetProfileQueryHandler 
    : IRequestHandler<GetProfileQuery, Result<UserProfileResponse>>
{
    // Implementación...
}
```

### 3. Mejorar Validaciones
```csharp
// Application/Features/Authentication/Register/RegisterCommandValidator.cs
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no es válido")
            .WithErrorCode("Email.Invalid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número")
            .WithErrorCode("Password.Invalid");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("El teléfono es requerido")
            .Matches(@"^\+?[1-9]\d{10,14}$").WithMessage("El formato del teléfono no es válido")
            .WithErrorCode("PhoneNumber.Invalid");
    }
}
```

### 4. Agregar Rate Limiting
```csharp
// API/DependencyInjection.cs
services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});
```

### 5. Agregar Response Caching
```csharp
// En endpoints que no cambien frecuentemente
[HttpGet("me")]
[ResponseCache(Duration = 60, VaryByHeader = "Authorization")]
public async Task<IActionResult> GetCurrentUser()
{
    // ...
}
```

### 6. Implementar Health Checks Detallados
```csharp
// API/HealthChecks/DatabaseHealthCheck.cs
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IApplicationDbContext _context;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is unhealthy", ex);
        }
    }
}
```

### 7. Agregar Correlación de Logs
```csharp
// API/Middleware/CorrelationIdMiddleware.cs
public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Response.Headers[CorrelationIdHeader] = correlationId;
        
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
```

## 📚 Documentación Adicional

- [ ] Crear OpenAPI/Swagger examples para cada DTO
- [ ] Documentar códigos de error en README
- [ ] Crear diagrams de flujo de autenticación
- [ ] Documentar contratos para el equipo de frontend

## ✅ Verificación Final

Una vez completado todo:

1. [ ] Compilar el proyecto sin errores
2. [ ] Ejecutar la aplicación
3. [ ] Probar todos los endpoints en Swagger
4. [ ] Verificar logs en `API/logs/`
5. [ ] Probar con Postman/Thunder Client
6. [ ] Verificar respuestas de error en diferentes ambientes
7. [ ] Revisar que TraceId se incluya en todas las respuestas de error
8. [ ] Confirmar que no se expone información sensible en producción

## 🎯 Criterios de Éxito

- ✅ Todos los DTOs están organizados por feature
- ✅ ErrorResponse sigue el estándar RFC 7807
- ✅ Middleware captura y maneja todas las excepciones
- ✅ TraceId permite correlacionar errores con logs
- ✅ Validaciones retornan errores descriptivos
- ✅ Comportamiento diferente entre dev y prod
- ✅ Documentación clara y completa
- ✅ Testing exitoso de todos los endpoints
