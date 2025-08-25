# SecurityStamp Implementation - Token Invalidation

## 🔐 O que é SecurityStamp?

O SecurityStamp é um campo único que invalida automaticamente tokens JWT quando informações sensíveis do usuário (como role) são alteradas.

## 🏗️ Como Funciona

### 1. **Campo SecurityStamp na Entidade User**
```csharp
public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

public void RefreshSecurityStamp()
{
    SecurityStamp = Guid.NewGuid().ToString();
}
```

### 2. **Inclusão no Token JWT**
```csharp
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Role, user.Role.ToString()),
    new Claim("SecurityStamp", user.SecurityStamp) // 🔑 Chave da segurança
};
```

### 3. **Validação em Cada Requisição**
O `SecurityStampValidationMiddleware` verifica se o SecurityStamp do token corresponde ao do banco:

```csharp
var securityStampClaim = context.User.FindFirst("SecurityStamp")?.Value;
var user = await userService.GetByIdAsync(userId);

if (user.SecurityStamp != securityStampClaim)
{
    // Token invalidado - usuário deve fazer login novamente
    context.Response.StatusCode = 401;
    return;
}
```

## 🔄 Fluxo de Invalidação

### Cenário: Alteração de Role
1. **Admin altera role de um usuário** (User → Admin ou Admin → User)
2. **Sistema chama `RefreshSecurityStamp()`** automaticamente
3. **Novo GUID é gerado** e salvo no banco
4. **Próxima requisição do usuário afetado**:
   - Middleware compara SecurityStamp do token vs banco
   - Como são diferentes, retorna **401 Unauthorized**
   - Usuário deve fazer login novamente para obter novo token

## 🚀 Vantagens

### ✅ **Segurança Imediata**
- Tokens são invalidados instantaneamente
- Não precisa esperar expiração do JWT

### ✅ **Automático**
- Não precisa manter lista de tokens revogados
- Funciona com qualquer mudança sensível

### ✅ **Escalável**
- Apenas uma consulta ao banco por requisição autenticada
- Performance adequada para a maioria das aplicações

## 📊 Casos de Uso

### 1. **Alteração de Role**
```csharp
// No UserController.Update()
if (existingUser.Role != user.Role)
{
    user.RefreshSecurityStamp(); // 🔄 Invalida tokens existentes
}
```

### 2. **Alteração de Senha**
```csharp
public async Task ChangePassword(int userId, string newPassword)
{
    user.PasswordHash = _authService.HashPassword(newPassword);
    user.RefreshSecurityStamp(); // 🔄 Invalida todos os tokens
    await _userService.UpdateAsync(user);
}
```

### 3. **Banimento/Suspensão**
```csharp
public async Task SuspendUser(int userId)
{
    user.IsActive = false;
    user.RefreshSecurityStamp(); // 🔄 Força logout imediato
    await _userService.UpdateAsync(user);
}
```

## 🛠️ Implementação Atual

### Arquivos Modificados:
- ✅ `User.cs` - Campo SecurityStamp + método RefreshSecurityStamp()
- ✅ `UserConfiguration.cs` - Configuração EF Core
- ✅ `AuthService.cs` - Inclusão do SecurityStamp no token
- ✅ `SecurityStampValidationMiddleware.cs` - Validação em cada requisição
- ✅ `UserController.cs` - Invalidação automática na alteração de role
- ✅ `Program.cs` - Registrado middleware após autenticação

### Migration Aplicada:
- ✅ `AddSecurityStamp` - Campo no banco de dados

## 🔧 Configuração

### Ordem dos Middlewares (Importante!):
```csharp
app.UseAuthentication();           // 1. Autentica o usuário
app.UseAuthorization();            // 2. Verifica permissões
app.UseMiddleware<SecurityStampValidationMiddleware>(); // 3. Valida SecurityStamp
app.UseMiddleware<AuthLoggingMiddleware>(); // 4. Log (opcional)
```

## 📝 Logs de Debug

O middleware gera logs úteis para troubleshooting:

```
- SecurityStamp validation passed for user {UserId}
- SecurityStamp mismatch for user {UserId}. Token: {TokenStamp}, DB: {DbStamp}
- Role mismatch for user {UserId}. Token role: {TokenRole}, DB role: {DbRole}
```

## 🧪 Como Testar

### 1. **Registre/Login um usuário**
```bash
POST /api/user/register
POST /api/user/login  # Obtenha o token
```

### 2. **Use o token em endpoints protegidos**
```bash
GET /api/user/profile  # Deve funcionar
```

### 3. **Altere o role do usuário** (como admin)
```bash
PUT /api/user/{id}  # Com role diferente
```

### 4. **Tente usar o token antigo**
```bash
GET /api/user/profile  # Deve retornar 401!
```

### 5. **Faça login novamente**
```bash
POST /api/user/login  # Novo token funciona
```

## 🎯 Resultado

**O sistema agora garante que qualquer alteração de role invalide imediatamente todos os tokens existentes do usuário, forçando um novo login para obter permissões atualizadas!**

Essa implementação oferece segurança robusta sem complexidade desnecessária, seguindo as melhores práticas de segurança em APIs REST.
