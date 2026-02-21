# 🏥 Health Checks - Guía Completa

## 📋 ¿Qué son los Health Checks?

Los **Health Checks** son endpoints que permiten monitorear el estado de tu API y sus dependencias en tiempo real. Son esenciales para:

- ✅ **Orquestadores** (Kubernetes, Docker Swarm): Saber cuándo reiniciar contenedores
- ✅ **Load Balancers**: Detectar instancias no saludables
- ✅ **Monitoreo**: Alertas automáticas cuando algo falla
- ✅ **Debugging**: Identificar problemas rápidamente

## 🎯 Estados de Health Check

| Estado | Código | Significado | Acción |
|--------|--------|-------------|--------|
| **Healthy** | 200 | ✅ Todo funciona | Ninguna |
| **Degraded** | 200 | ⚠️ Funciona pero con problemas | Investigar |
| **Unhealthy** | 503 | ❌ Servicio no disponible | Reiniciar/Alertar |

## 🔌 Endpoints Configurados

### 1. `/health` - Check Básico
**Uso:** Verificación rápida si la API responde

```bash
curl http://localhost:5000/health
```

**Respuesta:**
```
Healthy
```

---

### 2. `/health/details` - Información Completa
**Uso:** Ver estado detallado de todas las dependencias

```bash
curl http://localhost:5000/health/details
```

**Respuesta JSON:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0234567",
  "entries": {
    "sql-server": {
      "status": "Healthy",
      "duration": "00:00:00.0123456",
      "tags": ["db", "sql", "sqlserver"]
    },
    "ef-core-dbcontext": {
      "status": "Healthy",
      "duration": "00:00:00.0098765",
      "tags": ["db", "ef-core"]
    },
    "system-resources": {
      "status": "Healthy",
      "duration": "00:00:00.0001234",
      "data": {
        "MemoryUsedMB": 125,
        "MemoryLimitMB": 500,
        "MemoryUsagePercentage": 25.0,
        "CpuUsagePercentage": 15.5,
        "ThreadCount": 42,
        "HandleCount": 1234
      },
      "tags": ["system", "resources"]
    },
    "beefi-api": {
      "status": "Healthy",
      "data": {
        "Service": "BeeFi External API",
        "Status": "Configured"
      },
      "tags": ["external", "beefi"]
    }
  }
}
```

---

### 3. `/health/ready` - Readiness Check
**Uso:** Verifica que la base de datos está lista (para Kubernetes)

```bash
curl http://localhost:5000/health/ready
```

**Significado:**
- ✅ **Healthy**: La app puede recibir tráfico
- ❌ **Unhealthy**: La app NO debe recibir tráfico (DB no disponible)

---

### 4. `/health/live` - Liveness Check
**Uso:** Verifica que la aplicación está viva (sin dependencias)

```bash
curl http://localhost:5000/health/live
```

**Significado:**
- ✅ **Healthy**: La app está ejecutándose
- ❌ **Unhealthy**: La app debe reiniciarse (deadlock, crash)

---

## 🏗️ Health Checks Implementados

### 1️⃣ **sql-server** (Integrado)
- **Qué hace:** Ejecuta `SELECT 1;` en SQL Server
- **Detecta:** Problemas de conexión, DB caída, timeout
- **Tags:** `db`, `sql`, `sqlserver`

### 2️⃣ **ef-core-dbcontext** (Integrado)
- **Qué hace:** Verifica que EF Core puede conectarse y ejecutar queries
- **Detecta:** Problemas de DbContext, migraciones pendientes
- **Tags:** `db`, `ef-core`

### 3️⃣ **system-resources** (Custom)
- **Qué hace:** Monitorea memoria, CPU, threads
- **Configuración:**
  - Memoria límite: 500 MB
  - CPU límite: 90%
- **Detecta:** Memory leaks, alto uso de CPU
- **Tags:** `system`, `resources`

### 4️⃣ **beefi-api** (Custom)
- **Qué hace:** Verifica conectividad con API externa de BeeFi
- **Detecta:** API externa caída, timeout, problemas de red
- **Tags:** `external`, `beefi`

---

## 🐳 Integración con Docker/Kubernetes

### Docker Compose
```yaml
services:
  beefi-api:
    image: beefi-fruver-api:latest
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:80/health/ready"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
```

### Kubernetes Deployment
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: beefi-api
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: api
        image: beefi-fruver-api:latest
        # Verifica si el pod debe recibir tráfico
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
          initialDelaySeconds: 10
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3
        # Verifica si el pod está vivo (reinicia si falla)
        livenessProbe:
          httpGet:
            path: /health/live
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 30
          timeoutSeconds: 5
          failureThreshold: 3
```

---

## 📊 Monitoreo con Herramientas Externas

### 1. **Prometheus + Grafana**
```bash
# Instalar exportador de health checks
dotnet add package AspNetCore.HealthChecks.Publisher.Prometheus
```

### 2. **Azure Application Insights**
```csharp
builder.Services.AddHealthChecks()
    .AddApplicationInsightsPublisher();
```

### 3. **Datadog, New Relic, etc.**
Todos pueden consumir el endpoint `/health/details`

---

## 🛠️ Crear Health Checks Personalizados

### Ejemplo: Verificar Espacio en Disco
```csharp
public class DiskSpaceHealthCheck : IHealthCheck
{
    private readonly long _minimumFreeMB;

    public DiskSpaceHealthCheck(long minimumFreeMB = 1000)
    {
        _minimumFreeMB = minimumFreeMB;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var drive = new DriveInfo("C");
            var freeMB = drive.AvailableFreeSpace / 1024 / 1024;

            var data = new Dictionary<string, object>
            {
                { "DriveFreeMB", freeMB },
                { "MinimumRequiredMB", _minimumFreeMB }
            };

            if (freeMB < _minimumFreeMB)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Espacio en disco bajo: {freeMB}MB < {_minimumFreeMB}MB",
                    data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                "Espacio en disco suficiente",
                data: data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Error al verificar espacio en disco",
                exception: ex));
        }
    }
}
```

**Registrar en Program.cs:**
```csharp
builder.Services.AddHealthChecks()
    .AddCheck<DiskSpaceHealthCheck>(
        name: "disk-space",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "system", "disk" });
```

---

## ⚙️ Configuración Avanzada

### Timeouts Personalizados
```csharp
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: connectionString,
        healthQuery: "SELECT 1;",
        name: "sql-server",
        timeout: TimeSpan.FromSeconds(3) // Timeout de 3 segundos
    );
```

### Filtrar por Tags
```csharp
// Solo verificar checks de base de datos
app.MapHealthChecks("/health/database", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db")
});

// Solo verificar checks externos
app.MapHealthChecks("/health/external", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("external")
});
```

### Cache de Resultados
```csharp
builder.Services.AddHealthChecks()
    .AddCheck<ExpensiveHealthCheck>(
        name: "expensive-check",
        failureStatus: HealthStatus.Degraded)
    .AddCachedHealthCheck(
        name: "expensive-check-cached",
        cacheDuration: TimeSpan.FromMinutes(5)
    );
```

---

## 🧪 Testing en Desarrollo

### Powershell
```powershell
# Check básico
Invoke-RestMethod -Uri "http://localhost:5000/health"

# Check detallado con formato
Invoke-RestMethod -Uri "http://localhost:5000/health/details" | ConvertTo-Json -Depth 5

# Readiness check
Invoke-RestMethod -Uri "http://localhost:5000/health/ready"

# Liveness check
Invoke-RestMethod -Uri "http://localhost:5000/health/live"
```

### Postman
1. Crear colección "Health Checks"
2. Agregar requests para cada endpoint
3. Configurar tests:
```javascript
pm.test("Status is 200", function() {
    pm.response.to.have.status(200);
});

pm.test("Health status is Healthy", function() {
    var jsonData = pm.response.json();
    pm.expect(jsonData.status).to.eql("Healthy");
});
```

---

## 📈 Mejores Prácticas

### ✅ DO (Hacer)
- ✅ Usar health checks en producción
- ✅ Separar liveness y readiness
- ✅ Incluir timeouts cortos (3-5 segundos)
- ✅ Monitorear dependencias críticas (DB, APIs externas)
- ✅ Usar tags para filtrar checks
- ✅ Incluir datos útiles en los checks

### ❌ DON'T (No Hacer)
- ❌ Ejecutar operaciones costosas en health checks
- ❌ Usar health checks para business logic
- ❌ Exponer información sensible
- ❌ Tener health checks que siempre fallan
- ❌ Usar timeouts muy largos (>10s)

---

## 🚨 Troubleshooting

### Problema: Health check siempre retorna Unhealthy
**Solución:**
1. Verificar logs de Serilog
2. Revisar la conexión a DB
3. Ejecutar manualmente: `dotnet ef database update`

### Problema: Timeout en SQL Server check
**Solución:**
```csharp
.AddSqlServer(
    connectionString: connectionString,
    timeout: TimeSpan.FromSeconds(10) // Aumentar timeout
)
```

### Problema: El endpoint /health no responde
**Solución:**
- Verificar que `app.MapHealthChecks("/health")` esté configurado
- Verificar firewall/puerto

---

## 📚 Recursos Adicionales

- [Documentación oficial de ASP.NET Core Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [AspNetCore.Diagnostics.HealthChecks GitHub](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
- [Health Checks en Kubernetes](https://kubernetes.io/docs/tasks/configure-pod-container/configure-liveness-readiness-startup-probes/)
