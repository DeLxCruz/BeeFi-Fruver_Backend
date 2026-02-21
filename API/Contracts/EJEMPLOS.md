# 💡 Ejemplos de Uso - DTOs y Manejo de Errores

## 📥 Request Examples

### 1. RegisterRequest

```http
POST /api/authentication/register
Content-Type: application/json

{
  "email": "juan.perez@ejemplo.com",
  "password": "MiPassword123!",
  "firstName": "Juan Carlos",
  "lastName": "Pérez González",
  "phoneNumber": "+573001234567",
  "type": 0  // 0=Cliente, 1=Vendedor, 2=Repartidor, 3=Administrador
}
```

**Response Success (201 Created):**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "juan.perez@ejemplo.com",
  "firstName": "Juan Carlos",
  "lastName": "Pérez González",
  "roles": ["Cliente"],
  "hasBeeFiSubscription": false
}
```

**Response Error (400 Bad Request):**
```json
{
  "code": "User.EmailExists",
  "message": "Ya existe un usuario con este email",
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "0HN1234567890ABC",
  "path": "/api/authentication/register"
}
```

---

### 2. LoginRequest

```http
POST /api/authentication/login
Content-Type: application/json

{
  "email": "juan.perez@ejemplo.com",
  "password": "MiPassword123!"
}
```

**Response Success (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "d8f7e6c5b4a3928172635445fedc...",
  "expiresAt": "2024-01-15T11:00:00Z",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "juan.perez@ejemplo.com",
  "firstName": "Juan Carlos",
  "lastName": "Pérez González",
  "roles": ["Cliente"],
  "hasBeeFiSubscription": true,
  "beeFiPlanName": "Plan Premium",
  "discountPercentage": 15.0
}
```

**Response Error (401 Unauthorized):**
```json
{
  "code": "Authentication.InvalidCredentials",
  "message": "Email o contraseña incorrectos",
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "0HN1234567890ABC",
  "path": "/api/authentication/login"
}
```

---

### 3. RefreshTokenRequest

```http
POST /api/authentication/refresh-token
Content-Type: application/json

{
  "refreshToken": "d8f7e6c5b4a3928172635445fedc..."
}
```

**Response Success (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0k1l2m3n4...",
  "expiresAt": "2024-01-15T11:30:00Z",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "juan.perez@ejemplo.com",
  "firstName": "Juan Carlos",
  "lastName": "Pérez González",
  "roles": ["Cliente"],
  "hasBeeFiSubscription": true
}
```

**Response Error (401 Unauthorized):**
```json
{
  "code": "RefreshToken.Expired",
  "message": "El refresh token ha expirado",
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "0HN1234567890ABC",
  "path": "/api/authentication/refresh-token"
}
```

---

### 4. LogoutRequest

**Cerrar sesión en un dispositivo específico:**
```http
POST /api/authentication/logout
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "refreshToken": "d8f7e6c5b4a3928172635445fedc...",
  "revokeAllTokens": false
}
```

**Cerrar sesión en TODOS los dispositivos:**
```http
POST /api/authentication/logout
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "revokeAllTokens": true
}
```

**Response Success (200 OK):**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "tokensRevoked": 1,
  "message": "Se cerró sesión en este dispositivo"
}
```

---

## 🚨 Error Response Examples

### Validation Error (400 Bad Request)

```json
{
  "code": "Validation.Failed",
  "message": "Uno o más errores de validación ocurrieron",
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "0HN1234567890ABC",
  "path": "/api/authentication/register",
  "validationErrors": [
    {
      "field": "Email",
      "message": "El email no es válido",
      "code": "Email.Invalid",
      "attemptedValue": "invalid-email"
    },
    {
      "field": "Password",
      "message": "La contraseña debe tener al menos 8 caracteres",
      "code": "Password.TooShort",
      "attemptedValue": "123"
    },
    {
      "field": "PhoneNumber",
      "message": "El número de teléfono debe incluir código de país",
      "code": "PhoneNumber.InvalidFormat",
      "attemptedValue": "3001234567"
    }
  ]
}
```

### Unauthorized Error (401 Unauthorized)

```json
{
  "code": "Authorization.Unauthorized",
  "message": "No tienes autorización para acceder a este recurso",
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "0HN1234567890ABC",
  "path": "/api/authentication/me"
}
```

### Not Found Error (404 Not Found)

```json
{
  "code": "Resource.NotFound",
  "message": "Usuario no encontrado",
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "0HN1234567890ABC",
  "path": "/api/users/3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Internal Server Error (500)

**En Producción:**
```json
{
  "code": "Server.InternalError",
  "message": "Ocurrió un error interno en el servidor. Por favor, contacta al administrador.",
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "0HN1234567890ABC",
  "path": "/api/authentication/login"
}
```

**En Desarrollo:**
```json
{
  "code": "Server.InternalError",
  "message": "Object reference not set to an instance of an object.",
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "0HN1234567890ABC",
  "path": "/api/authentication/login",
  "details": {
    "message": "Object reference not set to an instance of an object.",
    "stackTrace": "   at Application.Features.Authentication.Login.LoginCommandHandler...",
    "innerException": null
  }
}
```

---

## 🔧 Código de Cliente (Flutter/JavaScript)

### JavaScript/TypeScript Example

```typescript
// types.ts
interface ErrorResponse {
  code: string;
  message: string;
  timestamp: string;
  traceId?: string;
  path?: string;
  validationErrors?: ValidationError[];
  details?: any;
}

interface ValidationError {
  field: string;
  message: string;
  code?: string;
  attemptedValue?: any;
}

// api-client.ts
async function login(email: string, password: string) {
  try {
    const response = await fetch('https://api.beefifruver.com/api/authentication/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
      const error: ErrorResponse = await response.json();
      
      // Manejar errores de validación
      if (error.validationErrors) {
        error.validationErrors.forEach(validationError => {
          console.error(`${validationError.field}: ${validationError.message}`);
        });
      }
      
      throw new Error(error.message);
    }

    const data = await response.json();
    
    // Guardar tokens
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    
    return data;
  } catch (error) {
    console.error('Login failed:', error);
    throw error;
  }
}
```

### Flutter/Dart Example

```dart
// models/error_response.dart
class ErrorResponse {
  final String code;
  final String message;
  final DateTime timestamp;
  final String? traceId;
  final String? path;
  final List<ValidationError>? validationErrors;

  ErrorResponse({
    required this.code,
    required this.message,
    required this.timestamp,
    this.traceId,
    this.path,
    this.validationErrors,
  });

  factory ErrorResponse.fromJson(Map<String, dynamic> json) {
    return ErrorResponse(
      code: json['code'],
      message: json['message'],
      timestamp: DateTime.parse(json['timestamp']),
      traceId: json['traceId'],
      path: json['path'],
      validationErrors: json['validationErrors'] != null
          ? (json['validationErrors'] as List)
              .map((e) => ValidationError.fromJson(e))
              .toList()
          : null,
    );
  }
}

class ValidationError {
  final String field;
  final String message;
  final String? code;
  final dynamic attemptedValue;

  ValidationError({
    required this.field,
    required this.message,
    this.code,
    this.attemptedValue,
  });

  factory ValidationError.fromJson(Map<String, dynamic> json) {
    return ValidationError(
      field: json['field'],
      message: json['message'],
      code: json['code'],
      attemptedValue: json['attemptedValue'],
    );
  }
}

// services/auth_service.dart
class AuthService {
  Future<LoginResponse> login(String email, String password) async {
    try {
      final response = await http.post(
        Uri.parse('https://api.beefifruver.com/api/authentication/login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'email': email,
          'password': password,
        }),
      );

      if (response.statusCode == 200) {
        return LoginResponse.fromJson(jsonDecode(response.body));
      } else {
        final errorResponse = ErrorResponse.fromJson(jsonDecode(response.body));
        
        // Manejar errores de validación
        if (errorResponse.validationErrors != null) {
          for (var error in errorResponse.validationErrors!) {
            print('${error.field}: ${error.message}');
          }
        }
        
        throw Exception(errorResponse.message);
      }
    } catch (e) {
      print('Login failed: $e');
      rethrow;
    }
  }
}
```

---

## 📊 Códigos de Error Comunes

| Código | HTTP Status | Descripción |
|--------|-------------|-------------|
| `User.EmailExists` | 400 | El email ya está registrado |
| `User.PhoneExists` | 400 | El teléfono ya está registrado |
| `User.NotFound` | 404 | Usuario no encontrado |
| `User.Inactive` | 401 | Usuario inactivo |
| `Authentication.InvalidCredentials` | 401 | Email o contraseña incorrectos |
| `Authentication.UserInactive` | 401 | Cuenta inactiva |
| `RefreshToken.Invalid` | 401 | Refresh token inválido |
| `RefreshToken.Expired` | 401 | Refresh token expirado |
| `RefreshToken.Revoked` | 401 | Refresh token revocado |
| `RefreshToken.NotFound` | 400 | Refresh token no encontrado |
| `Validation.Failed` | 400 | Errores de validación |
| `Authorization.Unauthorized` | 401 | Sin autorización |
| `Resource.NotFound` | 404 | Recurso no encontrado |
| `Operation.Invalid` | 400 | Operación inválida |
| `Argument.Invalid` | 400 | Argumento inválido |
| `Server.InternalError` | 500 | Error interno del servidor |

---

## 🧪 Testing con Postman/Thunder Client

### Environment Variables
```json
{
  "baseUrl": "https://localhost:7001",
  "accessToken": "",
  "refreshToken": "",
  "userId": ""
}
```

### Pre-request Script (Login)
```javascript
// Limpiar tokens antes de login
pm.environment.unset("accessToken");
pm.environment.unset("refreshToken");
```

### Test Script (Login)
```javascript
// Guardar tokens y userId
if (pm.response.code === 200) {
    const response = pm.response.json();
    pm.environment.set("accessToken", response.accessToken);
    pm.environment.set("refreshToken", response.refreshToken);
    pm.environment.set("userId", response.userId);
    
    console.log("✅ Login exitoso");
    console.log("UserId:", response.userId);
    console.log("HasBeeFi:", response.hasBeeFiSubscription);
} else {
    const error = pm.response.json();
    console.error("❌ Login falló");
    console.error("Code:", error.code);
    console.error("Message:", error.message);
    console.error("TraceId:", error.traceId);
}
```
