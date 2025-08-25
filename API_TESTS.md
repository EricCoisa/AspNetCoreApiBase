# CoreApiBase Authentication API Tests

## Base URL
```
http://localhost:5099
```

## 1. Register New User

### Request
```http
POST /api/user/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "TestPassword123!"
}
```

### Expected Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "testuser",
    "email": "test@example.com",
    "role": "User"
  },
  "message": "User registered successfully"
}
```

## 2. Login User

### Request
```http
POST /api/user/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "TestPassword123!"
}
```

### Expected Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "testuser",
    "email": "test@example.com",
    "role": "User"
  },
  "message": "Login successful"
}
```

## 3. Get User Profile (Authenticated)

### Request
```http
GET /api/user/profile
Authorization: Bearer {token-from-login}
```

### Expected Response
```json
{
  "id": 1,
  "username": "testuser",
  "email": "test@example.com",
  "role": "User"
}
```

## 4. Get All Users (Admin Only)

### Request
```http
GET /api/user
Authorization: Bearer {admin-token}
```

### Expected Response (if admin)
```json
[
  {
    "id": 1,
    "username": "testuser",
    "email": "test@example.com",
    "role": "User"
  }
]
```

### Expected Response (if not admin)
```json
Status: 403 Forbidden
```

## 5. Get User by ID

### Request
```http
GET /api/user/1
Authorization: Bearer {token}
```

### Expected Response (own data or admin)
```json
{
  "id": 1,
  "username": "testuser",
  "email": "test@example.com",
  "role": "User"
}
```

## 6. Create Admin User (Admin Only)

### Request
```http
POST /api/user
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "username": "admin",
  "email": "admin@example.com",
  "role": "Admin"
}
```

## cURL Examples

### Register User
```bash
curl -X POST "http://localhost:5099/api/user/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "TestPassword123!"
  }'
```

### Login User
```bash
curl -X POST "http://localhost:5099/api/user/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "TestPassword123!"
  }'
```

### Get Profile (replace TOKEN with actual token)
```bash
curl -X GET "http://localhost:5099/api/user/profile" \
  -H "Authorization: Bearer TOKEN"
```

### Get All Users (Admin only)
```bash
curl -X GET "http://localhost:5099/api/user" \
  -H "Authorization: Bearer ADMIN_TOKEN"
```

## Test Scenarios

### 1. Successful Registration Flow
1. Register new user → Should return 200 with token
2. Use token to get profile → Should return user data
3. Try to access admin endpoint → Should return 403

### 2. Authentication Flow
1. Login with valid credentials → Should return 200 with token
2. Login with invalid credentials → Should return 401
3. Access protected endpoint without token → Should return 401
4. Access protected endpoint with invalid token → Should return 401

### 3. Authorization Flow
1. User tries to access admin endpoint → Should return 403
2. User accesses own data → Should return 200
3. User tries to access other user's data → Should return 403
4. Admin accesses any data → Should return 200

### 4. Error Handling
1. Register with duplicate username → Should return 400
2. Register with duplicate email → Should return 400
3. Login with non-existent user → Should return 401
4. Access non-existent user by ID → Should return 404

## PowerShell Examples

### Register User
```powershell
$body = @{
    username = "testuser"
    email = "test@example.com"
    password = "TestPassword123!"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5099/api/user/register" -Method POST -Body $body -ContentType "application/json"
```

### Login User
```powershell
$body = @{
    username = "testuser"
    password = "TestPassword123!"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5099/api/user/login" -Method POST -Body $body -ContentType "application/json"
$token = $response.token
```

### Get Profile
```powershell
$headers = @{ Authorization = "Bearer $token" }
Invoke-RestMethod -Uri "http://localhost:5099/api/user/profile" -Method GET -Headers $headers
```
