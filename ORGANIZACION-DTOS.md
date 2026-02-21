# 📊 Resumen: Organización de DTOs y Manejo de Errores

## ✅ Cambios Implementados

### 1. **Estructura de Contratos (DTOs)**

Se creó una nueva estructura organizada en `API/Contracts/`:

```
API/
└── Contracts/
    ├── Common/
    │   └── ErrorResponse.cs         # ✅ Response de error estandarizado (RFC 7807)
    │
    └── Authentication/
        ├── RegisterRequest.cs       # ✅ Request para registro
        ├── LoginRequest.cs          # ✅ Request para login
        ├── RefreshTokenRequest.cs   # ✅ Request para renovar token
        ├── LogoutRequest.cs         # ✅ Request para logout
        └── UserProfileResponse.cs   # ✅ Response de perfil de usuario
```

### 2. **ErrorResponse Estandarizado (RFC 7807)**

El nuevo `ErrorResponse` sigue el estándar **RFC 7807 - Problem Details for HTTP APIs**:

```csharp
public record ErrorResponse
{
    public string Code { get; init; }                    // Código único del error
    public string Message { get; init; }                 // Mensaje descriptivo
    public DateTime Timestamp { get; init; }             // Cuándo ocurrió
    public string? TraceId { get; init; }               // Para rastreo en logs
    public string? Path { get; init; }                  // Endpoint donde falló
    public IEnumerable<ValidationError>? ValidationErrors { get; init; }  // Errores de validación
    public object? Details { get; init; }               // Info adicional (solo dev)
}
```

**Ejemplo de respuesta de error:**
```json
{
  "code": "User.EmailExists",
  "message": "Ya existe un usuario con este email",
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "0HN1234567890ABC",
  "path": "/api/authentication/register",
  "validationErrors": [
    {
      "field": "Email",
      "message": "El formato del email no es válido",
      "code": "Email.Invalid",
      "attemptedValue": "invalid-email"
    }
  ]
}
```

### 3. **GlobalExceptionHandlingMiddleware Mejorado**

Se implementó un middleware robusto que:

✅ **Captura todas las excepciones no manejadas**
✅ **Convierte excepciones a ErrorResponse estandarizado**
✅ **Maneja diferentes tipos de excepciones con códigos específicos:**
- `ValidationException` → 400 Bad Request
- `UnauthorizedAccessException` → 401 Unauthorized
- `KeyNotFoundException` → 404 Not Found
- `InvalidOperationException` → 400 Bad Request
- `ArgumentException` → 400 Bad Request
- Excepciones desconocidas → 500 Internal Server Error

✅ **Incluye información de debugging solo en desarrollo**
✅ **Registra todas las excepciones con Serilog**
✅ **Incluye TraceId para correlación con logs**

### 4. **AuthenticationController Actualizado**

El controlador ahora usa los nuevos contratos y el ErrorResponse estandarizado:

```csharp
// Antes
return BadRequest(new ErrorResponse(
    result.Error.Code,
    result.Error.Message,
    null
));

// Ahora
return BadRequest(new ErrorResponse(
    code: result.Error.Code,
    message: result.Error.Message,
    traceId: HttpContext.TraceIdentifier,
    path: HttpContext.Request.Path
));
```

## 📋 Organización de la Arquitectura

### Separación de Responsabilidades

```
┌────────────────────────────────────────────────────────────┐
│                        API LAYER                            │
│  - Controllers                                              │
│  - Contracts (DTOs): Requests/Responses                     │
│  - Middleware: Manejo de errores, seguridad, logging        │
└─────────────────┬──────────────────────────────────────────┘
                  │
                  │ Mapea DTOs → Commands/Queries
                  ▼
┌────────────────────────────────────────────────────────────┐
│                   APPLICATION LAYER                         │
│  - Features/                                                │
│    ├── Authentication/                                      │
│    │   ├── Login/                                           │
│    │   │   ├── LoginCommand.cs        (Input)              │
│    │   │   ├── LoginResponse.cs       (Output)             │
│    │   │   ├── LoginCommandHandler.cs (Lógica)             │
│    │   │   └── LoginCommandValidator.cs (Validación)       │
│    │   ├── Register/                                        │
│    │   ├── RefreshToken/                                    │
│    │   └── Logout/                                          │
│  - Common/                                                  │
│    ├── Behaviors/ (Pipeline behaviors)                      │
│    ├── Interfaces/                                          │
│    └── Models/ (DTOs compartidos internos)                  │
└─────────────────┬──────────────────────────────────────────┘
                  │
                  │ Usa entidades y primitivas
                  ▼
┌────────────────────────────────────────────────────────────┐
│                     DOMAIN LAYER                            │
│  - Entities/ (User, Product, Order, etc.)                   │
│  - Enums/ (UserType, OrderStatus, etc.)                     │
│  - Primitives/ (Result, Error, ValueObjects)                │
│  - Abstractions/ (Interfaces de dominio)                    │
└────────────────────────────────────────────────────────────┘
```

### Diferencias Clave

| Layer | Propósito | Ubicación | Ejemplos |
|-------|-----------|-----------|----------|
| **API Contracts** | DTOs para comunicación HTTP | `API/Contracts/` | `RegisterRequest`, `LoginRequest` |
| **Application Commands/Queries** | Casos de uso de negocio | `Application/Features/` | `LoginCommand`, `RegisterCommand` |
| **Application Responses** | Resultados de casos de uso | `Application/Features/` | `LoginResponse`, `RegisterResponse` |
| **Domain Entities** | Modelos de dominio | `Domain/Entities/` | `User`, `Product`, `Order` |

## 🎯 Ventajas de esta Organización

### 1. **Claridad y Mantenibilidad**
✅ Cada capa tiene responsabilidades bien definidas
✅ Fácil encontrar dónde agregar nuevos DTOs
✅ Organización por feature facilita escalabilidad

### 2. **Seguridad**
✅ No se exponen entidades de dominio al exterior
✅ Control total sobre qué información se envía al cliente
✅ Información sensible solo en desarrollo

### 3. **Estandarización**
✅ Todos los errores tienen el mismo formato
✅ TraceId permite correlacionar errores con logs
✅ Seguimiento del estándar RFC 7807

### 4. **Developer Experience**
✅ Documentación clara con XML comments
✅ Swagger generado automáticamente
✅ Intellisense completo
✅ Ejemplos en cada DTO

### 5. **Debugging**
✅ TraceId para rastreo de errores
✅ Path muestra dónde falló
✅ Timestamp para reproducir problemas
✅ ValidationErrors para errores específicos

## 🔄 Flujo de una Request

```
1. Cliente envía RegisterRequest
   ↓
2. Controller recibe RegisterRequest (API/Contracts)
   ↓
3. Controller crea RegisterCommand (Application)
   ↓
4. MediatR envía Command al Handler
   ↓
5. Handler valida con FluentValidation
   ↓
6. Handler ejecuta lógica de negocio
   ↓
7. Handler devuelve Result<RegisterResponse>
   ↓
8. Controller devuelve RegisterResponse o ErrorResponse
   ↓
9. Cliente recibe la respuesta JSON
```

## 🚨 Manejo de Errores - Flujo

```
┌─────────────┐
│ Exception   │
│ Lanzada     │
└──────┬──────┘
       │
       ▼
┌──────────────────────────────┐
│ GlobalExceptionHandling      │
│ Middleware                   │
│ - Captura la excepción       │
│ - Determina tipo             │
│ - Mapea a HTTP Status Code   │
│ - Crea ErrorResponse         │
│ - Incluye TraceId y Path     │
│ - Log con Serilog            │
└──────┬───────────────────────┘
       │
       ▼
┌──────────────────────────────┐
│ Cliente recibe ErrorResponse │
│ {                            │
│   "code": "...",             │
│   "message": "...",          │
│   "traceId": "...",          │
│   "path": "..."              │
│ }                            │
└──────────────────────────────┘
```

## 📝 Próximos Pasos Recomendados

1. **Crear DTOs para otros features:**
   - `API/Contracts/Products/`
   - `API/Contracts/Orders/`
   - `API/Contracts/Users/`

2. **Implementar GetUserProfile Query:**
   - `Application/Features/Authentication/GetProfile/`
   - Devolver UserProfileResponse completo

3. **Agregar más tipos de excepciones personalizadas:**
   - `DomainException` para errores de negocio
   - `NotFoundException<T>` genérica

4. **Configurar FluentValidation:**
   - Validators ya existen en Application
   - Asegurar que ValidationPipelineBehavior esté registrado

## 📚 Referencias

- [RFC 7807 - Problem Details for HTTP APIs](https://datatracker.ietf.org/doc/html/rfc7807)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [ASP.NET Core Best Practices](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)
