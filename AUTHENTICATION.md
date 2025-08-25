# Autenticação e Segurança - CoreApiBase

## Visão Geral

Esta API implementa um sistema de autenticação robusto usando JWT (JSON Web Tokens) com suporte a roles/claims, CORS configurável e preparação para OAuth2.

## Características de Segurança

### ✅ JWT Authentication
- **Algoritmo**: HMAC SHA-256
- **Expiração**: Configurável via `appsettings.json`
- **Claims**: UserId, Username, Email, Role
- **Validação**: Issuer, Audience, Lifetime, Signing Key

### ✅ Password Security
- **Hashing**: BCrypt com salt rounds = 12
- **Verificação**: Timing-safe comparison
- **Política**: Senhas hasheadas antes do armazenamento

### ✅ Authorization Policies
- **AdminOnly**: Apenas usuários com role Admin
- **UserOrAdmin**: Usuários normais ou Admin
- **Resource-based**: Usuários podem acessar apenas seus próprios dados

### ✅ CORS Configuration
- **Origins**: Configurável por ambiente
- **Methods**: Todos permitidos
- **Headers**: Todos permitidos
- **Credentials**: Habilitado

## Configuração

### appsettings.json
```json
{
  "JwtSettings": {
    "SecretKey": "SuaChaveSuperSecretaDeMinimo32Caracteres!@#$%",
    "Issuer": "CoreApiBase",
    "Audience": "CoreApiBase-Users",
    "ExpirationMinutes": 60
  },
  "CorsSettings": {
    "AllowedOrigins": ["http://localhost:3000", "https://meuapp.com"]
  }
}
```

### Variáveis de Ambiente (Produção)
```bash
JWT_SECRET_KEY=SuaChaveSuperSecretaEm Produção
JWT_ISSUER=CoreApiBase
JWT_AUDIENCE=CoreApiBase-Users
JWT_EXPIRATION_MINUTES=60
```

## Endpoints de Autenticação

### 🔓 POST `/api/user/register`
Registra um novo usuário no sistema.

**Request Body:**
```json
{
  "username": "johndoe",
  "email": "john@exemplo.com",
  "password": "MinhaSenh@123"
}
```

**Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "johndoe",
    "email": "john@exemplo.com",
    "role": "User"
  },
  "message": "User registered successfully"
}
```

### 🔓 POST `/api/user/login`
Autentica um usuário existente.

**Request Body:**
```json
{
  "username": "johndoe", // ou email
  "password": "MinhaSenh@123"
}
```

**Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "johndoe",
    "email": "john@exemplo.com",
    "role": "User"
  },
  "message": "Login successful"
}
```

### 🔒 GET `/api/user/profile`
Obtém o perfil do usuário autenticado.

**Headers:**
```
Authorization: Bearer {token}
```

**Response (200):**
```json
{
  "id": 1,
  "username": "johndoe",
  "email": "john@exemplo.com",
  "role": "User"
}
```

## Endpoints CRUD (Autorizados)

### 🔒👑 GET `/api/user`
Lista todos os usuários (apenas Admin).

**Headers:**
```
Authorization: Bearer {admin-token}
```

### 🔒 GET `/api/user/{id}`
Obtém um usuário específico (próprio usuário ou Admin).

### 🔒👑 POST `/api/user`
Cria um novo usuário (apenas Admin).

### 🔒 PUT `/api/user/{id}`
Atualiza um usuário (próprio usuário ou Admin).

### 🔒👑 DELETE `/api/user/{id}`
Remove um usuário (apenas Admin).

## Roles e Permissões

### User (Padrão)
- ✅ Ver próprio perfil
- ✅ Atualizar próprios dados
- ❌ Ver outros usuários
- ❌ Operações administrativas

### Admin
- ✅ Todas as operações de User
- ✅ Ver todos os usuários
- ✅ Criar usuários
- ✅ Atualizar qualquer usuário
- ✅ Deletar usuários

## Códigos de Status HTTP

| Código | Descrição |
|--------|-----------|
| 200 | Sucesso |
| 201 | Criado |
| 400 | Dados inválidos |
| 401 | Não autenticado |
| 403 | Sem permissão |
| 404 | Não encontrado |
| 500 | Erro interno |

## Exemplos de Uso

### Registrar e Fazer Login
```bash
# Registrar
curl -X POST "https://localhost:7000/api/user/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "email": "john@exemplo.com",
    "password": "MinhaSenh@123"
  }'

# Login
curl -X POST "https://localhost:7000/api/user/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "password": "MinhaSenh@123"
  }'
```

### Acessar Perfil
```bash
curl -X GET "https://localhost:7000/api/user/profile" \
  -H "Authorization: Bearer {seu-token}"
```

### Listar Usuários (Admin)
```bash
curl -X GET "https://localhost:7000/api/user" \
  -H "Authorization: Bearer {admin-token}"
```

## Preparação para OAuth2

A API está preparada para integração OAuth2:

### Placeholders Implementados
- `LoginWithGoogleAsync()` - Google OAuth2
- `LoginWithMicrosoftAsync()` - Microsoft OAuth2

### Próximos Passos OAuth2
1. Configurar chaves OAuth2 no `appsettings.json`
2. Implementar `GoogleOAuthService`
3. Implementar `MicrosoftOAuthService`
4. Adicionar endpoints OAuth2 no controller

## Logging de Autenticação

O middleware `AuthLoggingMiddleware` registra:
- ✅ Tentativas de login
- ✅ Tokens gerados
- ✅ Acessos autorizados
- ✅ Falhas de autenticação

## Segurança em Produção

### ⚠️ Checklist de Segurança
- [ ] Usar HTTPS em produção
- [ ] Configurar JWT Secret forte (>32 chars)
- [ ] Configurar CORS restritivo
- [ ] Implementar rate limiting
- [ ] Monitorar logs de autenticação
- [ ] Configurar backup do banco de dados
- [ ] Implementar rotação de chaves JWT

### Variáveis de Ambiente Recomendadas
```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=https://+:443;http://+:80
JWT_SECRET_KEY={chave-super-secreta-producao}
CORS_ORIGINS=https://meuapp.com,https://app.meudominio.com
```

## Troubleshooting

### Token Inválido (401)
- Verificar se o token não expirou
- Confirmar formato: `Bearer {token}`
- Validar chave JWT no `appsettings.json`

### Sem Permissão (403)
- Verificar role do usuário
- Confirmar policy de autorização
- Validar ownership de recursos

### CORS Errors
- Adicionar origin no `CorsSettings.AllowedOrigins`
- Verificar se `app.UseCors()` está configurado
- Confirmar ordem dos middlewares

---

**Desenvolvido com ❤️ para segurança e escalabilidade**
