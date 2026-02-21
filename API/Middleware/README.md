# 🛡️ Middlewares Personalizados

## Descripción General

Los middlewares se ejecutan en el siguiente orden (definido en `MiddlewareExtensions.cs`):

```
1. GlobalExceptionHandlingMiddleware  ← Captura todas las excepciones
2. SecurityHeadersMiddleware          ← Agrega headers de seguridad
3. RequestLoggingMiddleware           ← Logging detallado de requests
4. PerformanceMonitoringMiddleware    ← Monitoreo de performance
```

---

## 1️⃣ GlobalExceptionHandlingMiddleware

### Propósito
Captura todas las excepciones no manejadas y las convierte en respuestas HTTP estandarizadas según RFC 7807.

### Características
✅ Manejo centralizado de errores  
✅ Conversión automática a `ErrorResponse`  
✅ Logging automático con Serilog  
✅ TraceId para correlación con logs  
✅ Protección de información sensible en producción  

### Excepciones Manejadas
| Excepción | HTTP Status | Código |
|-----------|-------------|--------|
| `ValidationException` | 400 | `Validation.Failed` |
| `UnauthorizedAccessException` | 401 | `Authorization.Unauthorized` |
| `KeyNotFoundException` | 404 | `Resource.NotFound` |
| `InvalidOperationException` | 400 | `Operation.Invalid` |
| `ArgumentException` | 400 | `Argument.Invalid` |
| Otras | 500 | `Server.InternalError` |

### Ejemplo de Response

```json
{
  "code": "User.EmailExists",
  "message": "Ya existe un usuario con este email",
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "0HN1234567890ABC",
  "path": "/api/authentication/register",
  "validationErrors": null,
  "details": null
}
```

---

## 2️⃣ SecurityHeadersMiddleware

### Propósito
Agrega headers HTTP de seguridad para proteger contra vulnerabilidades comunes.

### Headers Agregados

#### X-Content-Type-Options: nosniff
**Protege contra:** MIME type sniffing  
**Descripción:** Previene que el navegador interprete archivos de forma diferente al Content-Type declarado.

#### X-Frame-Options: DENY
**Protege contra:** Clickjacking  
**Descripción:** Previene que tu sitio sea embebido en un iframe.

#### X-XSS-Protection: 1; mode=block
**Protege contra:** Cross-Site Scripting (XSS)  
**Descripción:** Activa la protección XSS del navegador (para navegadores legacy).

#### Referrer-Policy: strict-origin-when-cross-origin
**Protege contra:** Fuga de información  
**Descripción:** Controla qué información del referrer se envía.

#### Content-Security-Policy
**Protege contra:** XSS, Data Injection  
**Configuración:**
- `default-src 'self'` - Solo permite recursos del mismo origen
- `script-src 'self' 'unsafe-inline' 'unsafe-eval'` - Scripts
- `style-src 'self' 'unsafe-inline'` - Estilos
- `img-src 'self' data: https:` - Imágenes
- `connect-src 'self'` - APIs

#### Permissions-Policy
**Protege contra:** Uso no autorizado de APIs del navegador  
**Bloquea:** Camera, Microphone, Geolocation, Payment, USB, etc.

#### Strict-Transport-Security (Solo en HTTPS)
**Protege contra:** SSL Stripping, Downgrade attacks  
**Configuración:** `max-age=31536000; includeSubDomains`  
**Descripción:** Fuerza HTTPS por 1 año en todos los subdominios.

### Verificación

```bash
# Ver headers de seguridad
curl -I https://localhost:7001/api/health

# Output esperado:
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Referrer-Policy: strict-origin-when-cross-origin
Content-Security-Policy: default-src 'self'...
Permissions-Policy: accelerometer=()...
Strict-Transport-Security: max-age=31536000; includeSubDomains
```

---

## 3️⃣ RequestLoggingMiddleware

### Propósito
Proporciona logging detallado de cada request/response, complementando el logging automático de Serilog.

### Información Registrada
- ✅ Request ID (TraceIdentifier)
- ✅ HTTP Method (GET, POST, etc.)
- ✅ Path
- ✅ Status Code
- ✅ Tiempo de ejecución (milliseconds)
- ✅ Excepciones (si ocurren)

### Niveles de Log

**Request Iniciado** → `Information`
```log
[INF] Request 0HN1234567890ABC: POST /api/authentication/login started
```

**Request Exitoso** → `Information`
```log
[INF] Request 0HN1234567890ABC: POST /api/authentication/login completed with 200 in 145ms
```

**Request con Error** → `Error`
```log
[ERR] Request 0HN1234567890ABC: POST /api/authentication/login failed after 89ms
System.ArgumentException: Email is required
   at ...
```

### Correlación con Logs
Usa el mismo `TraceIdentifier` que `ErrorResponse`, permitiendo rastrear el flujo completo:

```
1. Request iniciado (RequestLoggingMiddleware)
2. Excepción lanzada
3. Error capturado (GlobalExceptionHandlingMiddleware)
4. Request fallido (RequestLoggingMiddleware)
```

---

## 4️⃣ PerformanceMonitoringMiddleware

### Propósito
Monitorea el rendimiento de cada request y alerta sobre requests lentos.

### Características
✅ Mide tiempo de respuesta de cada request  
✅ Alerta automática sobre requests lentos  
✅ Agrega header `X-Response-Time-Ms`  
✅ Threshold configurable en `appsettings.json`  

### Configuración

**appsettings.json:**
```json
{
  "Performance": {
    "SlowRequestThresholdMs": 1000  // Default: 1000ms
  }
}
```

**appsettings.Development.json:**
```json
{
  "Performance": {
    "SlowRequestThresholdMs": 500  // Más estricto en desarrollo
  }
}
```

**appsettings.Production.json:**
```json
{
  "Performance": {
    "SlowRequestThresholdMs": 2000  // Más tolerante en producción
  }
}
```

### Alertas de Performance

**Request Normal:**
```log
[INF] Request completed in 234ms
```

**Request Lento:**
```log
[WRN] ⚠️ SLOW REQUEST: POST /api/products/search took 1543ms (threshold: 1000ms) - Status: 200
```

### Header de Response Time

Cada response incluye un header con el tiempo de ejecución:

```http
HTTP/1.1 200 OK
Content-Type: application/json
X-Response-Time-Ms: 234
...
```

**Uso en el cliente:**
```javascript
const response = await fetch('/api/products');
const responseTime = response.headers.get('X-Response-Time-Ms');
console.log(`API response time: ${responseTime}ms`);
```

### Métricas y Optimización

Puedes usar estos logs para:
1. Identificar endpoints lentos
2. Detectar degradación de performance
3. Planificar optimizaciones
4. Monitorear el impacto de cambios

**Ejemplo de análisis:**
```bash
# Buscar requests lentos en logs
grep "SLOW REQUEST" logs/log-20240115.txt

# Output:
[WRN] SLOW REQUEST: POST /api/products/search took 1543ms
[WRN] SLOW REQUEST: GET /api/orders/123/details took 2105ms
[WRN] SLOW REQUEST: POST /api/authentication/register took 1234ms
```

---

## 🔄 Orden de Ejecución

### Pipeline Completo

```
CLIENT REQUEST
    ↓
┌─────────────────────────────────────────┐
│ 1. Serilog Request Logging              │ ← Logging automático
├─────────────────────────────────────────┤
│ 2. GlobalExceptionHandlingMiddleware    │ ← try/catch global
│    ↓                                     │
│    ┌─────────────────────────────────┐  │
│    │ 3. SecurityHeadersMiddleware    │  │ ← Agrega headers
│    │    ↓                            │  │
│    │    ┌────────────────────────┐   │  │
│    │    │ 4. RequestLogging      │   │  │ ← Log inicio
│    │    │    ↓                   │   │  │
│    │    │    ┌───────────────┐   │   │  │
│    │    │    │ 5. Performance│   │   │  │ ← Inicia timer
│    │    │    │    ↓          │   │   │  │
│    │    │    │    CONTROLLER │   │   │  │
│    │    │    │    ↓          │   │   │  │
│    │    │    └───────────────┘   │   │  │ ← Para timer, agrega header
│    │    │    ↓                   │   │  │
│    │    └────────────────────────┘   │  │ ← Log fin
│    │    ↓                            │  │
│    └─────────────────────────────────┘  │ ← Headers ya agregados
│    ↓                                     │
└─────────────────────────────────────────┘ ← catch si hay error
    ↓
CLIENT RESPONSE
```

### ¿Por qué este orden?

1. **GlobalExceptionHandlingMiddleware primero**
   - Captura excepciones de TODOS los middlewares siguientes
   - Garantiza que ningún error escape sin manejo

2. **SecurityHeadersMiddleware segundo**
   - Agrega headers antes de cualquier procesamiento
   - Headers presentes incluso si hay error

3. **RequestLoggingMiddleware tercero**
   - Log detallado del ciclo completo de request
   - Incluye información de todos los middlewares internos

4. **PerformanceMonitoringMiddleware último**
   - Mide el tiempo real de procesamiento
   - No incluye tiempo de logging externo

---

## 📊 Logs Completos - Ejemplo

### Request Exitoso

```log
[INF] Request 0HN1234567890ABC: POST /api/authentication/login started
[INF] HTTP POST /api/authentication/login responded 200 in 145.3456ms
[INF] Request 0HN1234567890ABC: POST /api/authentication/login completed with 200 in 145ms
```

### Request con Validación Error

```log
[INF] Request 0HN1234567890ABC: POST /api/authentication/register started
[WRN] Registration failed for email invalid@. Error: El email no es válido
[INF] HTTP POST /api/authentication/register responded 400 in 23.1234ms
[INF] Request 0HN1234567890ABC: POST /api/authentication/register completed with 400 in 23ms
```

### Request con Excepción

```log
[INF] Request 0HN1234567890ABC: POST /api/authentication/login started
[ERR] An unhandled exception occurred. TraceId: 0HN1234567890ABC, Path: /api/authentication/login
System.NullReferenceException: Object reference not set to an instance of an object.
   at Application.Features.Authentication.Login.LoginCommandHandler...
[ERR] Request 0HN1234567890ABC: POST /api/authentication/login failed after 89ms
[INF] HTTP POST /api/authentication/login responded 500 in 89.5678ms
```

### Request Lento

```log
[INF] Request 0HN1234567890ABC: GET /api/products/search started
[WRN] ⚠️ SLOW REQUEST: GET /api/products/search took 1543ms (threshold: 1000ms) - Status: 200
[INF] HTTP GET /api/products/search responded 200 in 1543.7890ms
[INF] Request 0HN1234567890ABC: GET /api/products/search completed with 200 in 1543ms
```

---

## 🔧 Configuración y Personalización

### Deshabilitar Middlewares (si es necesario)

```csharp
// MiddlewareExtensions.cs
public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app, bool enableSecurity = true)
{
    var builder = app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    
    if (enableSecurity)
        builder = builder.UseMiddleware<SecurityHeadersMiddleware>();
    
    return builder
        .UseMiddleware<RequestLoggingMiddleware>()
        .UseMiddleware<PerformanceMonitoringMiddleware>();
}
```

### Ajustar Content-Security-Policy

Para permitir más recursos (ej: CDNs):

```csharp
// SecurityHeadersMiddleware.cs
context.Response.Headers.Append(
    "Content-Security-Policy",
    "default-src 'self'; " +
    "script-src 'self' https://cdn.jsdelivr.net; " +
    "style-src 'self' https://fonts.googleapis.com; " +
    "font-src 'self' https://fonts.gstatic.com; " +
    "img-src 'self' data: https:; " +
    "connect-src 'self' https://api.external-service.com");
```

### Cambiar Threshold de Performance por Endpoint

```csharp
// En un Controller específico
[HttpGet("heavy-operation")]
[ResponseCache(Duration = 300)] // Cache 5 minutos
public async Task<IActionResult> HeavyOperation()
{
    // Esta operación puede tomar más tiempo
    // El threshold general sigue aplicando
}
```

---

## ✅ Verificación

### 1. Verificar que los middlewares estén registrados

```bash
# Iniciar la aplicación
dotnet run

# Buscar en logs de startup
grep "Middleware" logs/log-*.txt
```

### 2. Probar headers de seguridad

```bash
curl -I https://localhost:7001/api/health
```

### 3. Probar logging de requests

```bash
# Hacer request
curl https://localhost:7001/api/authentication/login -d '{"email":"test@test.com","password":"Test123!"}'

# Ver logs
tail -f logs/log-*.txt
```

### 4. Probar alertas de performance

```bash
# Crear un endpoint lento para testing
[HttpGet("slow")]
public async Task<IActionResult> Slow()
{
    await Task.Delay(2000); // 2 segundos
    return Ok();
}

# Hacer request
curl https://localhost:7001/api/test/slow

# Ver warning en logs
grep "SLOW REQUEST" logs/log-*.txt
```

---

## 🎯 Beneficios

✅ **Seguridad**: Headers protegen contra vulnerabilidades comunes  
✅ **Observabilidad**: Logging detallado de cada request  
✅ **Performance**: Identificación automática de problemas de rendimiento  
✅ **Debugging**: TraceId correlaciona errores con logs  
✅ **Mantenibilidad**: Código centralizado y reutilizable  
✅ **Estándares**: Sigue mejores prácticas de la industria  
