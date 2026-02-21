# 🏥 Health Checks UI - Guía de Solución de Problemas

## ❌ Problema: Página en Blanco en /health-ui

### Causas Comunes

1. **Falta `UseStaticFiles()`** ✅ RESUELTO
   - Health Checks UI necesita servir archivos estáticos (JS, CSS)
   - Solución: Agregado `app.UseStaticFiles()` en `Program.cs`

2. **URL incorrecta en configuración** ✅ RESUELTO
   - La URL debe ser completa (incluir protocolo y puerto)
   - Solución: Configurada en `appsettings.json`

3. **Certificado HTTPS no confiable**
   - El navegador puede bloquear la carga de recursos
   - Solución: Ver más abajo

## ✅ Configuración Actual

### appsettings.json
```json
{
  "HealthChecksUI": {
    "HealthChecks": [
      {
        "Name": "BeeFi Fruver API",
        "Uri": "https://localhost:7248/health/details"
      }
    ],
    "EvaluationTimeInSeconds": 10,
    "MinimumSecondsBetweenFailureNotifications": 60
  }
}
```

### Program.cs
```csharp
// Static Files (necesario para Health Checks UI)
app.UseStaticFiles();

// ...otros middlewares...

// Endpoints
app.MapControllers();
app.UsePresentation(); // Health Checks UI
```

## 🔧 Solución Completa

### 1. Confiar en el Certificado de Desarrollo

```bash
# PowerShell (como Administrador)
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### 2. Reiniciar la Aplicación

```bash
# Detener si está corriendo (Ctrl+C)
# Iniciar nuevamente
dotnet run
```

### 3. Verificar Endpoints

#### a) Health Check Básico
```bash
curl https://localhost:7248/health
```
**Respuesta esperada:**
```
Healthy
```

#### b) Health Check Detallado
```bash
curl https://localhost:7248/health/details
```
**Respuesta esperada:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "entries": {
    "sql-server": {
      "status": "Healthy",
      "duration": "00:00:00.0234567"
    },
    "ef-core-dbcontext": {
      "status": "Healthy",
      "duration": "00:00:00.0123456"
    }
  }
}
```

#### c) Health Checks UI Dashboard
Abrir en navegador:
```
https://localhost:7248/health-ui
```

#### d) Health Checks UI API (JSON)
```bash
curl https://localhost:7248/health-ui-api
```

## 🐛 Debugging

### Verificar en el Navegador

1. Abrir `https://localhost:7248/health-ui`
2. Presionar F12 para abrir DevTools
3. Ver la pestaña **Console** para errores de JavaScript
4. Ver la pestaña **Network** para ver si los recursos cargan

### Errores Comunes en Console

#### Error: "Mixed Content"
```
Mixed Content: The page at 'https://localhost:7248/health-ui' was loaded over HTTPS, 
but requested an insecure resource 'http://...'. This request has been blocked.
```

**Solución:** Asegurar que todo usa HTTPS, verificar `appsettings.json`

#### Error: "net::ERR_CERT_AUTHORITY_INVALID"
```
Failed to load resource: net::ERR_CERT_AUTHORITY_INVALID
```

**Solución:** Confiar en el certificado de desarrollo
```bash
dotnet dev-certs https --trust
```

#### Error: "Failed to fetch"
```
GET https://localhost:7248/health/details net::ERR_CONNECTION_REFUSED
```

**Solución:** La aplicación no está corriendo o el puerto es incorrecto

### Verificar Logs

```bash
# Ver logs en consola mientras la app corre
dotnet run

# O ver archivo de logs
cat logs/log-20241103.txt | Select-String -Pattern "health"
```

**Logs esperados:**
```log
[INF] Request 0HN...: GET /health-ui started
[INF] Request 0HN...: GET /health-ui completed with 200 in 45ms
[INF] Request 0HN...: GET /health-ui-api started
[INF] Request 0HN...: GET /health-ui-api completed with 200 in 12ms
```

## 📋 Checklist de Verificación

- [x] `UseStaticFiles()` agregado en `Program.cs`
- [x] URL completa en `appsettings.json` con protocolo y puerto correcto
- [x] `AddHealthChecksUI()` registrado en servicios
- [x] `MapHealthChecksUI()` mapeado en endpoints
- [ ] Certificado HTTPS confiable (ejecutar `dotnet dev-certs https --trust`)
- [ ] Aplicación corriendo sin errores
- [ ] Base de datos accesible (SQL Server corriendo)
- [ ] Endpoint `/health/details` responde correctamente

## 🎯 Resultado Esperado

Al navegar a `https://localhost:7248/health-ui` deberías ver:

1. **Encabezado:** "Health Checks UI"
2. **Lista de endpoints:** "BeeFi Fruver API"
3. **Estado:** Verde (Healthy) o Rojo (Unhealthy)
4. **Checks individuales:**
   - sql-server: ✅ Healthy
   - ef-core-dbcontext: ✅ Healthy
   - system-resources: ✅ Healthy
   - beefi-api: ⚠️ Degraded (si la API externa no está disponible)

## 🔄 Si Aún No Funciona

### Opción 1: Usar HTTP en lugar de HTTPS (solo desarrollo)

**appsettings.Development.json:**
```json
{
  "HealthChecksUI": {
    "HealthChecks": [
      {
        "Name": "BeeFi Fruver API",
        "Uri": "http://localhost:5298/health/details"
      }
    ]
  }
}
```

Luego navegar a: `http://localhost:5298/health-ui`

### Opción 2: Verificar que el endpoint /health/details funcione

```bash
# PowerShell
Invoke-WebRequest -Uri "https://localhost:7248/health/details" -SkipCertificateCheck
```

### Opción 3: Ver logs detallados

**appsettings.Development.json:**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "HealthChecks.UI": "Debug"
      }
    }
  }
}
```

## 📚 Más Información

- [Health Checks UI Documentation](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
- [ASP.NET Core Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
