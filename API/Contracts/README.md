# 📦 API Contracts (DTOs)

Esta carpeta contiene todos los **Data Transfer Objects (DTOs)** utilizados en la capa de API para la comunicación con los clientes.

## 📁 Estructura Organizacional

```
Contracts/
├── Common/                    # DTOs compartidos entre múltiples features
│   └── ErrorResponse.cs      # Response de error estandarizado (RFC 7807)
│
└── Authentication/           # DTOs específicos de autenticación
    ├── RegisterRequest.cs    # Request para registro de usuarios
    ├── LoginRequest.cs       # Request para login
    ├── RefreshTokenRequest.cs # Request para renovar token
    ├── LogoutRequest.cs      # Request para logout
    └── UserProfileResponse.cs # Response con perfil de usuario
```

## 🎯 Principios de Diseño

### 1. **Separación de Responsabilidades**
- **Contracts (API Layer)**: DTOs simples para entrada/salida HTTP
- **Commands/Queries (Application Layer)**: Lógica de negocio y validaciones
- **Entities (Domain Layer)**: Modelos de dominio

### 2. **Organización por Feature**
Los DTOs están organizados por **característica/módulo** (Feature-based):
- `Authentication/` - Registro, login, tokens
- `Products/` - Gestión de productos
- `Orders/` - Pedidos y carritos
- etc.

### 3. **Nomenclatura Clara**
- **Request**: Datos de entrada del cliente → `[Action]Request`
  - Ejemplo: `RegisterRequest`, `LoginRequest`
- **Response**: Datos de salida al cliente → `[Action]Response`
  - Ejemplo: `LoginResponse`, `UserProfileResponse`

## ✅ Estándar de Manejo de Errores (RFC 7807)

### ErrorResponse
Basado en **RFC 7807 - Problem Details for HTTP APIs**

```csharp
{
  "code": "User.EmailExists",              // Código único del error
  "message": "El email ya está registrado", // Mensaje descriptivo
  "timestamp": "2024-01-15T10:30:00Z",     // Cuándo ocurrió
  "traceId": "0HN1234567890ABC",            // Para rastreo en logs
  "path": "/api/authentication/register",   // Endpoint donde falló
  "validationErrors": [                     // Errores de validación (opcional)
    {
      "field": "Email",
      "message": "El email no es válido",
      "code": "Email.Invalid",
      "attemptedValue": "invalid-email"
    }
  ],
  "details": { }                            // Información adicional (solo en dev)
}
```

### Ventajas de este estándar:
✅ **Consistencia**: Todas las respuestas de error tienen el mismo formato  
✅ **Trazabilidad**: TraceId permite correlacionar errores con logs  
✅ **Debugging**: Path y Timestamp ayudan a reproducir problemas  
✅ **Validación Clara**: ValidationErrors separa errores de validación  
✅ **Seguridad**: Details solo se muestra en desarrollo  

## 🔧 Cómo Usar los Contratos

### En Controllers
```csharp
[HttpPost("register")]
[ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    var command = new RegisterCommand(
        request.Email,
        request.Password,
        request.FirstName,
        request.LastName,
        request.PhoneNumber,
        request.Type);

    var result = await _mediator.Send(command);

    if (result.IsFailure)
    {
        return BadRequest(new ErrorResponse(
            code: result.Error.Code,
            message: result.Error.Message,
            traceId: HttpContext.TraceIdentifier,
            path: HttpContext.Request.Path
        ));
    }

    return CreatedAtAction(nameof(Register), result.Value);
}
```

## 📋 Convenciones

### 1. Usar `record` en lugar de `class`
```csharp
// ✅ Correcto
public record LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

// ❌ Evitar
public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
```

**Razones:**
- Inmutabilidad por defecto (`init`)
- Comparación por valor automática
- Sintaxis más concisa
- Mejora performance

### 2. Propiedades con `init` y valores por defecto
```csharp
public string Email { get; init; } = string.Empty;
```

### 3. Documentación XML
```csharp
/// <summary>
/// Request para autenticar un usuario
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// Email del usuario
    /// </summary>
    /// <example>usuario@ejemplo.com</example>
    public string Email { get; init; } = string.Empty;
}
```

## 🚀 Buenas Prácticas

### ✅ DO's
- ✅ Organizar DTOs por feature/módulo
- ✅ Usar `record` para inmutabilidad
- ✅ Documentar con XML comments
- ✅ Validar en Application layer (Commands/Queries)
- ✅ Incluir ejemplos en documentación
- ✅ Usar ErrorResponse estandarizado

### ❌ DON'Ts
- ❌ No incluir lógica de negocio en DTOs
- ❌ No usar DTOs directamente en Domain layer
- ❌ No exponer entidades de dominio al cliente
- ❌ No crear DTOs genéricos reutilizados en todo
- ❌ No incluir información sensible en responses

## 🔄 Flujo de Datos

```
┌─────────────┐
│   Cliente   │
│  (Request)  │
└──────┬──────┘
       │ RegisterRequest
       ▼
┌─────────────────┐
│  Controller     │ ──────────┐
│  (API Layer)    │            │ Mapea a
└────────┬────────┘            │
         │                     ▼
         │              RegisterCommand
         │                     │
         ▼                     ▼
    ┌─────────────────────────────┐
    │  MediatR Handler            │
    │  (Application Layer)        │
    │  - Validación               │
    │  - Lógica de negocio        │
    └────────┬────────────────────┘
             │
             ▼
       RegisterResponse
             │
             ▼
┌─────────────────┐
│   Cliente       │
│  (Response)     │
└─────────────────┘
```

## 📚 Referencias

- [RFC 7807 - Problem Details](https://datatracker.ietf.org/doc/html/rfc7807)
- [REST API Best Practices](https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design)
- [Clean Architecture - DTOs](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
