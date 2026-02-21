# 🔄 Orden del Middleware Pipeline en ASP.NET Core

## ⚠️ ORDEN CRÍTICO

El orden de los middlewares en ASP.NET Core es **EXTREMADAMENTE IMPORTANTE**. Un orden incorrecto puede causar errores en tiempo de ejecución o comportamientos inesperados.

## 📋 Orden Correcto en BeeFi Fruver API

```csharp
// 1. Exception Handling (PRIMERO para capturar TODO)
app.UseSerilogRequestLogging();
app.UseCustomMiddlewares();  // Incluye GlobalExceptionHandlingMiddleware

// 2. HTTPS Redirection
app.UseHttpsRedirection();

// 3. Static Files (si se usan)
// app.UseStaticFiles();

// 4. CORS (ANTES de Routing)
app.UseCors("PolicyName");

// 5. Routing (OBLIGATORIO antes de Authentication/Authorization/Endpoints)
app.UseRouting();

// 6. Authentication (DESPUÉS de Routing, ANTES de Authorization)
app.UseAuthentication();

// 7. Authorization (DESPUÉS de Authentication, ANTES de Endpoints)
app.UseAuthorization();

// 8. Custom Middleware que necesita usuario autenticado
// app.UseCustomUserMiddleware();

// 9. Endpoints (SIEMPRE AL FINAL)
app.MapControllers();
app.UseEndpoints(...);  // Health Checks, etc.
```

## 🚨 Errores Comunes

### 1. UseEndpoints sin UseRouting

**❌ Error:**
```
EndpointRoutingMiddleware matches endpoints setup by EndpointMiddleware 
and so must be added to the request execution pipeline before EndpointMiddleware.
```

**✅ Solución:**
```csharp
app.UseRouting();  // ← Agregar ANTES de UseEndpoints
app.MapControllers();
app.UseEndpoints(...);
```

### 2. UseAuthentication después de UseAuthorization

**❌ Incorrecto:**
```csharp
app.UseAuthorization();
app.UseAuthentication();  // ← Orden incorrecto
```

**✅ Correcto:**
```csharp
app.UseAuthentication();  // ← Primero autenticación
app.UseAuthorization();   // ← Luego autorización
```

### 3. CORS después de Routing

**❌ Incorrecto:**
```csharp
app.UseRouting();
app.UseCors("Policy");  // ← Muy tarde
```

**✅ Correcto:**
```csharp
app.UseCors("Policy");  // ← Antes de Routing
app.UseRouting();
```

### 4. Exception Handler al final

**❌ Incorrecto:**
```csharp
app.UseRouting();
app.UseAuthentication();
app.UseExceptionHandler(...);  // ← No capturará excepciones anteriores
```

**✅ Correcto:**
```csharp
app.UseExceptionHandler(...);  // ← PRIMERO
app.UseRouting();
app.UseAuthentication();
```

## 📊 Pipeline Completo de BeeFi Fruver

### Flujo de Request → Response

```
┌─────────────────────────────────────────────────────────────────┐
│                      CLIENT REQUEST                              │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│ 1. Serilog Request Logging                                       │
│    - Log inicio de request                                       │
│    - Enriquece con IP, User-Agent, etc.                          │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│ 2. GlobalExceptionHandlingMiddleware (try/catch global)          │
│    ┌───────────────────────────────────────────────────────┐    │
│    │ 3. SecurityHeadersMiddleware                          │    │
│    │    - Agrega headers de seguridad                      │    │
│    │    ┌─────────────────────────────────────────────┐    │    │
│    │    │ 4. RequestLoggingMiddleware                 │    │    │
│    │    │    - Log detallado del request              │    │    │
│    │    │    ┌───────────────────────────────────┐    │    │    │
│    │    │    │ 5. PerformanceMonitoringMiddleware│    │    │    │
│    │    │    │    - Inicia timer                 │    │    │    │
│    │    │    │    ┌─────────────────────────┐    │    │    │    │
│    │    │    │    │ 6. HTTPS Redirection    │    │    │    │    │
│    │    │    │    └────────┬────────────────┘    │    │    │    │
│    │    │    │             │                     │    │    │    │
│    │    │    │             ▼                     │    │    │    │
│    │    │    │    ┌─────────────────────────┐    │    │    │    │
│    │    │    │    │ 7. CORS                 │    │    │    │    │
│    │    │    │    │    - Valida origen      │    │    │    │    │
│    │    │    │    │    - Agrega headers     │    │    │    │    │
│    │    │    │    └────────┬────────────────┘    │    │    │    │
│    │    │    │             │                     │    │    │    │
│    │    │    │             ▼                     │    │    │    │
│    │    │    │    ┌─────────────────────────┐    │    │    │    │
│    │    │    │    │ 8. Routing              │    │    │    │    │
│    │    │    │    │    - Match endpoint     │    │    │    │    │
│    │    │    │    └────────┬────────────────┘    │    │    │    │
│    │    │    │             │                     │    │    │    │
│    │    │    │             ▼                     │    │    │    │
│    │    │    │    ┌─────────────────────────┐    │    │    │    │
│    │    │    │    │ 9. Authentication       │    │    │    │    │
│    │    │    │    │    - Valida JWT         │    │    │    │    │
│    │    │    │    │    - Crea ClaimsPrincipal│   │    │    │    │
│    │    │    │    └────────┬────────────────┘    │    │    │    │
│    │    │    │             │                     │    │    │    │
│    │    │    │             ▼                     │    │    │    │
│    │    │    │    ┌─────────────────────────┐    │    │    │    │
│    │    │    │    │ 10. Authorization       │    │    │    │    │
│    │    │    │    │     - Valida roles      │    │    │    │    │
│    │    │    │    │     - Valida policies   │    │    │    │    │
│    │    │    │    └────────┬────────────────┘    │    │    │    │
│    │    │    │             │                     │    │    │    │
│    │    │    │             ▼                     │    │    │    │
│    │    │    │    ┌─────────────────────────┐    │    │    │    │
│    │    │    │    │ 11. CONTROLLER          │    │    │    │    │
│    │    │    │    │     - Ejecuta action    │    │    │    │    │
│    │    │    │    │     - MediatR handler   │    │    │    │    │
│    │    │    │    │     - Business logic    │    │    │    │    │
│    │    │    │    └────────┬────────────────┘    │    │    │    │
│    │    │    │             │ Response            │    │    │    │
│    │    │    │             ▼                     │    │    │    │
│    │    │    │    - Para timer                   │    │    │    │
│    │    │    │    - Agrega X-Response-Time-Ms    │    │    │    │
│    │    │    └─────────────┬───────────────────┘    │    │    │
│    │    │                  │                         │    │    │
│    │    │                  ▼                         │    │    │
│    │    │    - Log fin de request                    │    │    │
│    │    └──────────────────┬───────────────────────┘    │    │
│    │                       │                             │    │
│    │                       ▼                             │    │
│    │    - Headers de seguridad ya agregados             │    │
│    └───────────────────────┬─────────────────────────────┘    │
│                            │                                   │
│                            ▼                                   │
│    - catch si hay excepción → ErrorResponse                    │
└────────────────────────────┬───────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│ Serilog Request Logging (log final)                             │
│ - Log respuesta con status code y tiempo                        │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                      CLIENT RESPONSE                             │
└─────────────────────────────────────────────────────────────────┘
```

## 🔍 Detalles por Middleware

### Serilog Request Logging
- **Posición:** Inicio del pipeline
- **Por qué:** Captura TODOS los requests, incluso los que fallan
- **Qué hace:** Log automático con tiempo de respuesta

### GlobalExceptionHandlingMiddleware
- **Posición:** Segundo (dentro de try/catch)
- **Por qué:** Envuelve todo el resto del pipeline
- **Qué hace:** Captura excepciones y las convierte a ErrorResponse

### SecurityHeadersMiddleware
- **Posición:** Tercero
- **Por qué:** Headers deben agregarse temprano
- **Qué hace:** Agrega headers de seguridad HTTP

### RequestLoggingMiddleware
- **Posición:** Cuarto
- **Por qué:** Log antes y después de procesamiento
- **Qué hace:** Log detallado con TraceId

### PerformanceMonitoringMiddleware
- **Posición:** Quinto
- **Por qué:** Mide tiempo real de procesamiento
- **Qué hace:** Timer y alertas de performance

### HTTPS Redirection
- **Posición:** Antes de CORS
- **Por qué:** Redirigir a HTTPS antes de validar origen
- **Qué hace:** HTTP → HTTPS redirect

### CORS
- **Posición:** Antes de Routing
- **Por qué:** Validar origen antes de ruteo
- **Qué hace:** Valida origen, agrega headers CORS

### Routing
- **Posición:** Antes de Authentication
- **Por qué:** Necesario para identificar endpoint
- **Qué hace:** Match request a endpoint específico

### Authentication
- **Posición:** Después de Routing, antes de Authorization
- **Por qué:** Necesita saber el endpoint, identifica usuario
- **Qué hace:** Valida JWT, crea ClaimsPrincipal

### Authorization
- **Posición:** Después de Authentication, antes de Controller
- **Por qué:** Necesita usuario identificado
- **Qué hace:** Valida roles y policies

### Endpoints (Controllers)
- **Posición:** Al final
- **Por qué:** Ejecuta lógica después de todas las validaciones
- **Qué hace:** Business logic, MediatR, Database access

## 📝 Ejemplo Completo

```csharp
var app = builder.Build();

// ========================================
// 🔧 MIDDLEWARE PIPELINE (ORDEN CRÍTICO)
// ========================================

// 1. Development Tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Logging (PRIMERO)
app.UseSerilogRequestLogging();

// 3. Exception Handling (SEGUNDO - envuelve todo)
app.UseCustomMiddlewares();

// 4. HTTPS Redirection
app.UseHttpsRedirection();

// 5. Static Files (si se usan)
app.UseStaticFiles();

// 6. CORS (ANTES de Routing)
app.UseCors("PolicyName");

// 7. Routing (OBLIGATORIO antes de Auth)
app.UseRouting();

// 8. Authentication (DESPUÉS de Routing)
app.UseAuthentication();

// 9. Authorization (DESPUÉS de Authentication)
app.UseAuthorization();

// 10. Rate Limiting (si se usa)
app.UseRateLimiter();

// 11. Endpoints (SIEMPRE AL FINAL)
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

## ✅ Checklist de Verificación

- [ ] `UseSerilogRequestLogging()` está al inicio
- [ ] `UseCustomMiddlewares()` (Exception Handler) está temprano
- [ ] `UseCors()` está ANTES de `UseRouting()`
- [ ] `UseRouting()` existe y está ANTES de `UseAuthentication()`
- [ ] `UseAuthentication()` está ANTES de `UseAuthorization()`
- [ ] `UseAuthorization()` está ANTES de `MapControllers()`
- [ ] `MapControllers()` y `UseEndpoints()` están al final

## 🔗 Referencias

- [ASP.NET Core Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware)
- [Middleware Order](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/#middleware-order)
- [Routing](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing)
