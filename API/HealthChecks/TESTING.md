# 🧪 Testing Health Checks - Comandos Rápidos

## 🚀 Iniciar la API
```powershell
cd c:\Users\delxc\Documents\BeeFi\BeeFi-Fruver\Backend\BeeFi-Fruver\API
dotnet run
```

## 📝 Probar Health Checks (PowerShell)

### 1. Health Check Básico
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/health"
```
**Salida esperada:** `Healthy`

---

### 2. Health Check Detallado (JSON Completo)
```powershell
$response = Invoke-RestMethod -Uri "http://localhost:5000/health/details"
$response | ConvertTo-Json -Depth 10
```

**Salida esperada:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0234567",
  "entries": {
    "sql-server": {
      "status": "Healthy",
      "duration": "00:00:00.0123456"
    },
    "ef-core-dbcontext": {
      "status": "Healthy"
    },
    "system-resources": {
      "status": "Healthy",
      "data": {
        "MemoryUsedMB": 125,
        "CpuUsagePercentage": 15.5
      }
    },
    "beefi-api": {
      "status": "Healthy"
    }
  }
}
```

---

### 3. Readiness Check (Para Kubernetes)
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/health/ready"
```
**Qué verifica:** Solo base de datos (tags: "db")

---

### 4. Liveness Check
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/health/live"
```
**Qué verifica:** Si la app está viva (sin dependencias)

---

## 🌐 Probar con Navegador

Abre en tu navegador:
- http://localhost:5000/health
- http://localhost:5000/health/details
- http://localhost:5000/health/ready
- http://localhost:5000/health/live

---

## 🔄 Loop de Monitoreo Continuo

```powershell
# Monitoreo cada 5 segundos
while ($true) {
    Clear-Host
    Write-Host "=== Health Check @ $(Get-Date -Format 'HH:mm:ss') ===" -ForegroundColor Cyan
    
    try {
        $health = Invoke-RestMethod -Uri "http://localhost:5000/health/details"
        
        Write-Host "`nEstado General: " -NoNewline
        if ($health.status -eq "Healthy") {
            Write-Host "✅ $($health.status)" -ForegroundColor Green
        } elseif ($health.status -eq "Degraded") {
            Write-Host "⚠️  $($health.status)" -ForegroundColor Yellow
        } else {
            Write-Host "❌ $($health.status)" -ForegroundColor Red
        }
        
        Write-Host "Duración Total: $($health.totalDuration)`n"
        
        Write-Host "Checks Individuales:" -ForegroundColor Yellow
        foreach ($entry in $health.entries.PSObject.Properties) {
            $name = $entry.Name
            $value = $entry.Value
            $status = $value.status
            $duration = $value.duration
            
            $emoji = if ($status -eq "Healthy") { "✅" } 
                     elseif ($status -eq "Degraded") { "⚠️" } 
                     else { "❌" }
            
            Write-Host "  $emoji $name : $status ($duration)"
            
            # Mostrar datos si existen
            if ($value.data) {
                foreach ($data in $value.data.PSObject.Properties) {
                    Write-Host "     - $($data.Name): $($data.Value)" -ForegroundColor Gray
                }
            }
        }
    }
    catch {
        Write-Host "❌ Error al conectar con la API" -ForegroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor DarkRed
    }
    
    Start-Sleep -Seconds 5
}
```

**Para detener:** Presiona `Ctrl+C`

---

## 🐳 Probar con Docker (cuando tengas Dockerfile)

```bash
# Build
docker build -t beefi-fruver-api .

# Run con healthcheck
docker run -d \
  --name beefi-api \
  --health-cmd="curl -f http://localhost/health || exit 1" \
  --health-interval=30s \
  --health-timeout=10s \
  --health-retries=3 \
  -p 5000:80 \
  beefi-fruver-api

# Ver estado del healthcheck
docker ps
docker inspect beefi-api | grep -A 20 Health
```

---

## 📊 Integración con Postman

### Crear Colección
1. Crear nueva colección "BeeFi - Health Checks"
2. Agregar estos requests:

#### Request 1: Basic Health
- **Method:** GET
- **URL:** `{{baseUrl}}/health`
- **Tests:**
```javascript
pm.test("Status code is 200", function() {
    pm.response.to.have.status(200);
});

pm.test("Response is Healthy", function() {
    pm.expect(pm.response.text()).to.include("Healthy");
});
```

#### Request 2: Detailed Health
- **Method:** GET
- **URL:** `{{baseUrl}}/health/details`
- **Tests:**
```javascript
pm.test("Status code is 200", function() {
    pm.response.to.have.status(200);
});

pm.test("Overall status is Healthy", function() {
    var jsonData = pm.response.json();
    pm.expect(jsonData.status).to.eql("Healthy");
});

pm.test("SQL Server is healthy", function() {
    var jsonData = pm.response.json();
    pm.expect(jsonData.entries["sql-server"].status).to.eql("Healthy");
});

pm.test("All checks passed", function() {
    var jsonData = pm.response.json();
    for (const [key, value] of Object.entries(jsonData.entries)) {
        pm.expect(value.status).to.be.oneOf(["Healthy", "Degraded"]);
    }
});
```

#### Request 3: Readiness
- **Method:** GET
- **URL:** `{{baseUrl}}/health/ready`

#### Request 4: Liveness
- **Method:** GET
- **URL:** `{{baseUrl}}/health/live`

### Variables de Entorno
```json
{
  "baseUrl": "http://localhost:5000"
}
```

---

## 🎯 Casos de Prueba

### ✅ Caso 1: Todo funcionando
```powershell
# Ejecutar con DB disponible
dotnet run
Invoke-RestMethod -Uri "http://localhost:5000/health/details"
# Resultado esperado: status = "Healthy"
```

### ⚠️ Caso 2: Base de datos caída
```powershell
# 1. Detener SQL Server (Services -> SQL Server)
# 2. Ejecutar health check
Invoke-RestMethod -Uri "http://localhost:5000/health/details"
# Resultado esperado: status = "Unhealthy", sql-server = "Unhealthy"
```

### 🔄 Caso 3: Alta carga de memoria (simulado)
```powershell
# Modificar SystemResourcesHealthCheck.cs
# Cambiar: maxMemoryMegabytes = 1 (muy bajo)
# Recompilar y ejecutar
dotnet run
Invoke-RestMethod -Uri "http://localhost:5000/health/details"
# Resultado esperado: system-resources = "Unhealthy"
```

---

## 📈 Monitoreo en Producción

### Azure Application Insights
```powershell
# Consulta KQL
requests
| where url contains "/health/details"
| where timestamp > ago(1h)
| summarize count() by resultCode
```

### Prometheus Query
```promql
# Rate de health checks fallidos
rate(health_check_status{status="Unhealthy"}[5m])
```

### Alertas
```yaml
# Alert Rule Example
alert: APIHealthCheckFailed
expr: health_check_status{name="sql-server"} != 1
for: 5m
annotations:
  summary: "BeeFi API Database Health Check Failed"
  description: "SQL Server health check has been failing for 5 minutes"
```

---

## 🛠️ Troubleshooting

### Problema: 404 Not Found en /health
**Causa:** El middleware no está registrado
**Solución:**
```csharp
// Verificar en Program.cs
app.MapHealthChecks("/health");
```

### Problema: Timeout en SQL Server check
**Causa:** Conexión lenta o DB sobrecargada
**Solución:** Aumentar timeout
```csharp
.AddSqlServer(
    connectionString: connectionString,
    timeout: TimeSpan.FromSeconds(10)
)
```

### Problema: Health check siempre Unhealthy
**Causa:** Error en el check personalizado
**Solución:** Ver logs de Serilog
```powershell
# Ver logs
Get-Content .\logs\log-*.txt -Tail 50
```
