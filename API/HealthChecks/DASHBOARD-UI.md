# 🎨 Health Checks UI Dashboard - Guía de Acceso

## 🚀 Cómo Iniciar y Acceder al Dashboard

### 1️⃣ Iniciar la API

```powershell
cd c:\Users\delxc\Documents\BeeFi\BeeFi-Fruver\Backend\BeeFi-Fruver\API
dotnet run
```

**Salida esperada:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

### 2️⃣ Abrir el Dashboard en el Navegador

**URL del Dashboard:**
```
http://localhost:5000/health-ui
```

o si usas HTTPS:
```
https://localhost:5001/health-ui
```

---

## 🖼️ Qué Verás en el Dashboard

### Pantalla Principal

El dashboard mostrará:

1. **Estado General** (Grande en la parte superior)
   - 🟢 **Healthy** - Todo funciona correctamente
   - 🟡 **Degraded** - Funciona pero con advertencias
   - 🔴 **Unhealthy** - Servicios críticos caídos

2. **Lista de Health Checks Monitoreados**
   - BeeFi Fruver API
     - sql-server
     - ef-core-dbcontext
     - system-resources
     - beefi-api

3. **Información Detallada por Check**
   - Estado actual
   - Última verificación
   - Duración de la verificación
   - Datos adicionales (memoria, CPU, etc.)

4. **Gráficas de Historial**
   - Evolución en el tiempo
   - Últimas 50 evaluaciones
   - Tendencias de disponibilidad

---

## 📋 Endpoints Disponibles

| Endpoint | Descripción | Para Qué |
|----------|-------------|----------|
| `/health-ui` | **Dashboard visual** | Ver en navegador (humanos) |
| `/health-ui-api` | API del dashboard | Consumo por otras herramientas |
| `/health` | Check simple | Scripts, load balancers |
| `/health/details` | JSON detallado | Debugging, APIs |
| `/health/ready` | Readiness (DB) | Kubernetes |
| `/health/live` | Liveness | Kubernetes |

---

## 🎯 Capturas de Pantalla del Dashboard

### Estado Healthy (Todo OK) ✅
```
╔════════════════════════════════════════════════╗
║  BeeFi Fruver API Health Checks               ║
║                                                ║
║  Status: ● Healthy                             ║
║  Last Check: 26/10/2025 15:30:45              ║
║  Next Check: 10 seconds                        ║
╠════════════════════════════════════════════════╣
║  Health Checks:                                ║
║  ✅ sql-server           Healthy   12ms       ║
║  ✅ ef-core-dbcontext    Healthy   15ms       ║
║  ✅ system-resources     Healthy    2ms       ║
║  ✅ beefi-api           Healthy    5ms       ║
╚════════════════════════════════════════════════╝
```

### Estado Unhealthy (Problemas) ❌
```
╔════════════════════════════════════════════════╗
║  BeeFi Fruver API Health Checks               ║
║                                                ║
║  Status: ● Unhealthy                           ║
║  Last Check: 26/10/2025 15:32:10              ║
╠════════════════════════════════════════════════╣
║  Health Checks:                                ║
║  ❌ sql-server           Unhealthy  timeout   ║
║  ❌ ef-core-dbcontext    Unhealthy  timeout   ║
║  ✅ system-resources     Healthy    2ms       ║
║  ⚠️  beefi-api           Degraded   150ms     ║
╚════════════════════════════════════════════════╝
```

---

## ⚙️ Configuración del Dashboard

### En `appsettings.json`:

```json
{
  "HealthChecksUI": {
    "HealthChecks": [
      {
        "Name": "BeeFi Fruver API",
        "Uri": "http://localhost:5000/health/details"
      }
    ],
    "EvaluationTimeInSeconds": 10,      // ⏰ Evalúa cada 10 segundos
    "MinimumSecondsBetweenFailureNotifications": 60  // 🔔 Espera 60s entre alertas
  }
}
```

### En `Program.cs`:

```csharp
builder.Services
    .AddHealthChecksUI(setup =>
    {
        setup.SetEvaluationTimeInSeconds(10);
        setup.MaximumHistoryEntriesPerEndpoint(50);  // 📊 Guarda 50 evaluaciones
        setup.AddHealthCheckEndpoint("BeeFi Fruver API", "/health/details");
    })
    .AddInMemoryStorage();  // 💾 Almacenamiento en memoria
```

---

## 🔄 Monitorear Múltiples APIs

Si tienes varios servicios, puedes monitorearlos todos:

```json
{
  "HealthChecksUI": {
    "HealthChecks": [
      {
        "Name": "BeeFi Fruver API",
        "Uri": "http://localhost:5000/health/details"
      },
      {
        "Name": "BeeFi Payment Service",
        "Uri": "http://localhost:5001/health/details"
      },
      {
        "Name": "BeeFi Notification Service",
        "Uri": "http://localhost:5002/health/details"
      }
    ]
  }
}
```

---

## 🎨 Personalizar el Dashboard

### Cambiar la URL del Dashboard

```csharp
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/dashboard";       // Nuevo: /dashboard
    options.ApiPath = "/dashboard-api";
});
```

Acceder en: `http://localhost:5000/dashboard`

### Configurar Webhooks (Notificaciones)

```csharp
builder.Services
    .AddHealthChecksUI(setup =>
    {
        setup.SetEvaluationTimeInSeconds(10);
        setup.AddWebhookNotification("teams",
            uri: "https://outlook.office.com/webhook/...",
            payload: "{'text': 'Health check failed!'}",
            restorePayload: "{'text': 'Health check restored!'}");
    })
    .AddInMemoryStorage();
```

---

## 📊 Almacenamiento Persistente (SQL Server)

Si quieres que el historial se mantenga al reiniciar:

### 1. Instalar paquete
```powershell
dotnet add package AspNetCore.HealthChecks.UI.SqlServer.Storage
```

### 2. Configurar en Program.cs
```csharp
builder.Services
    .AddHealthChecksUI()
    .AddSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
```

### 3. Crear tablas (automático al iniciar)
Las tablas se crean automáticamente:
- `HealthCheckExecutionHistory`
- `HealthCheckFailureNotifications`
- `HealthCheckConfigurations`

---

## 🧪 Testing del Dashboard

### 1. Verificar que la API está corriendo
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/health"
```

### 2. Verificar el endpoint de detalles
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/health/details"
```

### 3. Abrir el Dashboard
```
http://localhost:5000/health-ui
```

### 4. Verificar actualización automática
- El dashboard se actualiza cada 10 segundos automáticamente
- Verás un spinner/loading indicator antes de cada actualización

---

## 🚨 Troubleshooting

### Problema: Dashboard muestra "No data"
**Causa:** El endpoint `/health/details` no responde

**Solución:**
```powershell
# Verificar que el endpoint funciona
Invoke-RestMethod -Uri "http://localhost:5000/health/details"
```

### Problema: Dashboard no se actualiza
**Causa:** La configuración de `EvaluationTimeInSeconds` puede ser muy alta

**Solución:**
1. Revisar `appsettings.json`
2. Reducir `EvaluationTimeInSeconds` a 5 segundos
3. Reiniciar la API

### Problema: 404 en /health-ui
**Causa:** No está mapeado el endpoint

**Solución:**
Verificar en `Program.cs`:
```csharp
app.MapHealthChecksUI();
```

### Problema: Dashboard muy lento
**Causa:** Demasiados checks o evaluación muy frecuente

**Solución:**
```json
{
  "HealthChecksUI": {
    "EvaluationTimeInSeconds": 30  // Aumentar de 10 a 30
  }
}
```

---

## 📱 Acceso desde Otros Dispositivos

### En la misma red local:

1. Obtener tu IP local:
```powershell
ipconfig | Select-String "IPv4"
```

2. Configurar `launchSettings.json` para escuchar en todas las interfaces:
```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://0.0.0.0:5000"
    }
  }
}
```

3. Acceder desde otro dispositivo:
```
http://TU_IP:5000/health-ui
```
Ejemplo: `http://192.168.1.100:5000/health-ui`

---

## 🎉 Características del Dashboard

### ✅ Auto-Refresh
- Se actualiza automáticamente cada X segundos
- No necesitas recargar la página

### 📊 Gráficas
- Historial de estado
- Tiempo de respuesta
- Disponibilidad (uptime)

### 🔔 Notificaciones
- Webhooks a Teams, Slack, Discord
- Emails (con configuración adicional)

### 📈 Métricas
- Promedio de tiempo de respuesta
- Porcentaje de disponibilidad
- Fallos consecutivos

### 🎨 Responsive
- Funciona en desktop y móvil
- Modo oscuro/claro (según tema del navegador)

---

## 🚀 Siguientes Pasos

1. ✅ **Iniciar la API**: `dotnet run`
2. ✅ **Abrir Dashboard**: `http://localhost:5000/health-ui`
3. 🎯 **Explorar las características**
4. 📊 **Monitorear tu API en tiempo real**

---

## 📚 Recursos

- [Documentación oficial](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
- [Ejemplos avanzados](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/tree/master/samples)
- [Webhooks configuration](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/wiki/HealthCheck-UI-Webhooks)

---

## 🎊 ¡Disfruta tu Dashboard!

Ahora tienes un **dashboard visual profesional** para monitorear el estado de tu API en tiempo real. 

**¡Es hora de ejecutarlo!** 🚀
