# 📋 Sistema de Aprobación de Cuentas - Guía Completa

## ✅ Implementación Completada

### 🎯 Características Implementadas

1. ✅ **Enum AccountStatus** - Estados de cuenta (Pending, Approved, Rejected, Suspended)
2. ✅ **Entidad User actualizada** - Campos de aprobación agregados
3. ✅ **RegisterCommandHandler** - FruverAliados requieren aprobación automáticamente
4. ✅ **LoginCommandHandler** - Validación de estado de cuenta
5. ✅ **Features de Admin** - Aprobar, Rechazar, Suspender usuarios
6. ✅ **Controller de Admin** - Endpoints REST para administración
7. ✅ **Notificaciones por Email** - Avisos automáticos a usuarios

---

## 🔄 Flujo Completo del Sistema

### 1️⃣ Registro de Usuario

#### Cliente Normal
```http
POST /api/authentication/register
{
  "email": "cliente@example.com",
  "password": "Password123!",
  "firstName": "Juan",
  "lastName": "Pérez",
  "phoneNumber": "3001234567",
  "type": "Cliente"
}
```

**Resultado:**
- ✅ `AccountStatus = Approved` (aprobado automáticamente)
- ✅ Puede iniciar sesión inmediatamente
- 📧 Recibe email de confirmación

#### FruverAliado (Vendedor)
```http
POST /api/authentication/register
{
  "email": "vendedor@fruver.com",
  "password": "Password123!",
  "firstName": "María",
  "lastName": "González",
  "phoneNumber": "3001234568",
  "type": "FruverAliado"
}
```

**Resultado:**
- ⏳ `AccountStatus = Pending` (pendiente de aprobación)
- ❌ NO puede iniciar sesión hasta ser aprobado
- 📧 Recibe email: "Tu cuenta está pendiente de aprobación"

---

### 2️⃣ Intento de Login

#### Usuario Pendiente
```http
POST /api/authentication/login
{
  "email": "vendedor@fruver.com",
  "password": "Password123!"
}
```

**Respuesta:**
```json
{
  "isSuccess": false,
  "error": {
    "code": "Authentication.PendingApproval",
    "message": "Tu cuenta está pendiente de aprobación por el administrador"
  }
}
```

#### Usuario Rechazado
```json
{
  "isSuccess": false,
  "error": {
    "code": "Authentication.AccountRejected",
    "message": "Tu cuenta fue rechazada. Razón: No cumple con los requisitos"
  }
}
```

#### Usuario Suspendido
```json
{
  "isSuccess": false,
  "error": {
    "code": "Authentication.AccountSuspended",
    "message": "Tu cuenta está suspendida. Razón: Violación de términos"
  }
}
```

---

### 3️⃣ Panel de Administración

## 📊 Ver Usuarios Pendientes

```http
GET /api/admin/users/pending
Authorization: Bearer {admin-jwt-token}
```

**Respuesta:**
```json
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "vendedor@fruver.com",
    "firstName": "María",
    "lastName": "González",
    "phoneNumber": "3001234568",
    "profileImageUrl": null,
    "accountStatus": "Pending",
    "createdAt": "2025-11-03T10:00:00Z",
    "daysPending": 2
  }
]
```

---

## ✅ Aprobar Usuario

```http
POST /api/admin/users/{userId}/approve
Authorization: Bearer {admin-jwt-token}
```

**Resultado:**
- ✅ `AccountStatus = Approved`
- ✅ `ApprovedAt = DateTime.UtcNow`
- ✅ `ApprovedBy = AdminUserId`
- ✅ `IsActive = true`
- 📧 Usuario recibe email: "¡Tu cuenta ha sido aprobada!"

**Response:**
```json
{
  "message": "Usuario aprobado exitosamente"
}
```

---

## ❌ Rechazar Usuario

```http
POST /api/admin/users/{userId}/reject
Authorization: Bearer {admin-jwt-token}
Content-Type: application/json

{
  "reason": "No cumple con los requisitos de documentación"
}
```

**Resultado:**
- ❌ `AccountStatus = Rejected`
- ❌ `IsActive = false`
- 📝 `RejectionReason = "No cumple con los requisitos..."`
- 📧 Usuario recibe email con la razón del rechazo

**Response:**
```json
{
  "message": "Usuario rechazado exitosamente"
}
```

---

## ⏸️ Suspender Usuario

```http
POST /api/admin/users/{userId}/suspend
Authorization: Bearer {admin-jwt-token}
Content-Type: application/json

{
  "reason": "Violación de términos de servicio"
}
```

**Resultado:**
- ⏸️ `AccountStatus = Suspended`
- ❌ `IsActive = false`
- 📝 `RejectionReason = "Violación de términos..."`
- 📧 Usuario recibe email notificando la suspensión

**Response:**
```json
{
  "message": "Usuario suspendido exitosamente"
}
```

---

## 🗄️ Estructura de Base de Datos

### Tabla Users - Nuevos Campos

```sql
ALTER TABLE Users ADD
    AccountStatus int NOT NULL DEFAULT 1,  -- 0=Pending, 1=Approved, 2=Rejected, 3=Suspended
    RejectionReason nvarchar(500) NULL,
    ApprovedAt datetime2 NULL,
    ApprovedBy uniqueidentifier NULL;

CREATE INDEX IX_Users_AccountStatus ON Users(AccountStatus);
```

---

## 🎭 Casos de Uso

### Caso 1: Nuevo Fruver Aliado se Registra
1. Fruver se registra → `AccountStatus = Pending`
2. Admin revisa en `/api/admin/users/pending`
3. Admin aprueba → Usuario recibe email
4. Fruver puede iniciar sesión y publicar productos

### Caso 2: Cliente se Registra
1. Cliente se registra → `AccountStatus = Approved` (automático)
2. Cliente puede iniciar sesión inmediatamente
3. No requiere aprobación de admin

### Caso 3: Usuario Problemático
1. Admin detecta comportamiento inadecuado
2. Admin suspende → `POST /api/admin/users/{id}/suspend`
3. Usuario NO puede iniciar sesión
4. Usuario recibe email con razón

### Caso 4: Usuario Intenta Login Pendiente
1. Fruver intenta login antes de aprobación
2. Sistema retorna error: "Pendiente de aprobación"
3. Fruver no puede acceder hasta aprobación

---

## 🔐 Seguridad

### Endpoints de Admin Protegidos
```csharp
[Authorize(Roles = "Administrador")]
public class AdminUsersController : ControllerBase
```

Solo usuarios con rol **Administrador** pueden:
- ✅ Ver usuarios pendientes
- ✅ Aprobar usuarios
- ✅ Rechazar usuarios
- ✅ Suspender usuarios

---

## 📧 Emails Enviados

### 1. Registro de FruverAliado
**Asunto:** Cuenta pendiente de aprobación - BeeFi  
**Contenido:** Notificación de que su cuenta será revisada

### 2. Cuenta Aprobada
**Asunto:** ¡Tu cuenta ha sido aprobada! - BeeFi  
**Contenido:** Enlace para iniciar sesión y comenzar

### 3. Cuenta Rechazada
**Asunto:** Actualización de tu solicitud - BeeFi  
**Contenido:** Razón del rechazo y contacto de soporte

### 4. Cuenta Suspendida
**Asunto:** Tu cuenta ha sido suspendida - BeeFi  
**Contenido:** Razón de suspensión y contacto de soporte

---

## 🧪 Cómo Probar

### 1. Registrar FruverAliado
```bash
curl -X POST https://localhost:7248/api/authentication/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "fruver@test.com",
    "password": "Password123!",
    "firstName": "Test",
    "lastName": "Fruver",
    "phoneNumber": "3001234567",
    "type": "FruverAliado"
  }'
```

### 2. Intentar Login (debería fallar)
```bash
curl -X POST https://localhost:7248/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "fruver@test.com",
    "password": "Password123!"
  }'
```

### 3. Login como Admin
```bash
# Primero necesitas crear un usuario Admin en la BD
# O usar el endpoint de register con tipo "Administrador"
```

### 4. Ver Pendientes (como Admin)
```bash
curl -X GET https://localhost:7248/api/admin/users/pending \
  -H "Authorization: Bearer {admin-token}"
```

### 5. Aprobar Usuario (como Admin)
```bash
curl -X POST https://localhost:7248/api/admin/users/{userId}/approve \
  -H "Authorization: Bearer {admin-token}"
```

### 6. Login de FruverAliado (ahora debería funcionar)
```bash
curl -X POST https://localhost:7248/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "fruver@test.com",
    "password": "Password123!"
  }'
```

---

## 📝 Notas Importantes

1. **Clientes** se aprueban automáticamente (`AccountStatus = Approved`)
2. **FruverAliados** requieren aprobación manual (`AccountStatus = Pending`)
3. **Empleados** se aprueban automáticamente
4. Solo el rol **Administrador** puede gestionar aprobaciones
5. Los emails se envían de forma asíncrona (fire and forget)
6. El sistema registra quién aprobó/rechazó cada usuario

---

## 🚀 Próximas Mejoras

- [ ] Dashboard de administración en el frontend
- [ ] Notificaciones push cuando se aprueba/rechaza
- [ ] Historial de cambios de estado
- [ ] Razones predefinidas para rechazo/suspensión
- [ ] Proceso de apelación para usuarios rechazados
- [ ] Métricas de tiempo de aprobación

---

## 🆘 Troubleshooting

### "Usuario no encontrado"
- Verifica que el `userId` sea correcto
- Verifica que el usuario exista en la base de datos

### "La cuenta no está pendiente de aprobación"
- Solo se pueden aprobar usuarios con `AccountStatus = Pending`
- Verifica el estado actual con `GET /api/admin/users/pending`

### "No se pudo identificar al administrador actual"
- Verifica que el token JWT sea válido
- Verifica que el usuario tenga rol "Administrador"

---

**¡Sistema de aprobación de cuentas completamente funcional!** ✨
