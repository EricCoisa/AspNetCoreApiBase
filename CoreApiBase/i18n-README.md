# Internationalization (i18n) Documentation

This document explains how to use the internationalization feature implemented in CoreApiBase.

## Supported Languages

- **en-US** (English - Default)
- **pt-BR** (Portuguese - Brazil)
- **es-ES** (Spanish - Spain)

## How to Select Language

### 1. Via Query String
Add `?culture=pt-BR` to any API endpoint URL:
```
GET /api/user/1?culture=pt-BR
GET /api/authentication/profile?culture=es-ES
```

### 2. Via Accept-Language Header
Send the Accept-Language HTTP header:
```
Accept-Language: pt-BR
Accept-Language: es-ES
Accept-Language: en-US
```

## Available Localized Messages

The following messages are localized across all supported languages:

### User Messages
- `UserNotFound` - User not found / Usuário não encontrado / Usuario no encontrado
- `ErrorRetrievingUsers` - Error retrieving users / Erro ao buscar usuários / Error al obtener usuarios
- `ErrorRetrievingUser` - Error retrieving user / Erro ao buscar usuário / Error al obtener usuario
- `ErrorCreatingUser` - Error creating user / Erro ao criar usuário / Error al crear usuario
- `ErrorUpdatingUser` - Error updating user / Erro ao atualizar usuário / Error al actualizar usuario
- `ErrorDeletingUser` - Error deleting user / Erro ao excluir usuário / Error al eliminar usuario

### Authentication Messages
- `InvalidCredentials` - Invalid credentials / Credenciais inválidas / Credenciales inválidas
- `UserExistsAlready` - Username or email already exists / Nome de usuário ou email já existe / Nombre de usuario o email ya existe
- `UserRegisteredSuccessfully` - User registered successfully / Usuário registrado com sucesso / Usuario registrado exitosamente
- `LoginSuccessful` - Login successful / Login realizado com sucesso / Inicio de sesión exitoso
- `TokensRevoked` - Tokens revoked for user / Tokens revogados para o usuário / Tokens revocados para el usuario

### General Messages
- `Forbidden` - Access forbidden / Acesso negado / Acceso denegado
- `BadRequest` - Bad request / Solicitação inválida / Solicitud inválida

## How to Add New Languages

1. Create new resource files in `CoreApiBase/Resources/`:
   - `SharedResource.{culture}.resx` (e.g., `SharedResource.fr-FR.resx` for French)

2. Add the culture to the supported cultures list in `Program.cs`:
   ```csharp
   var supportedCultures = new[] { "en-US", "pt-BR", "es-ES", "fr-FR" };
   ```

3. Add all the required localized strings to the new .resx file.

## How to Add New Localized Messages

1. Add the key-value pair to all resource files:
   - `SharedResource.resx` (English - default)
   - `SharedResource.pt-BR.resx` (Portuguese)
   - `SharedResource.es-ES.resx` (Spanish)

2. Use in controllers via dependency injection:
   ```csharp
   private readonly IStringLocalizer<SharedResource> _localizer;
   
   // In constructor
   public MyController(IStringLocalizer<SharedResource> localizer)
   {
       _localizer = localizer;
   }
   
   // Usage
   return NotFound(new { message = _localizer["MyNewMessage"] });
   
   // With parameters
   return StatusCode(500, new { message = _localizer["ErrorMessage", ex.Message] });
   ```

## Example API Requests

### English (Default)
```bash
curl -X GET "https://localhost:7001/api/user/999"
# Response: {"message": "User not found."}
```

### Portuguese
```bash
curl -X GET "https://localhost:7001/api/user/999?culture=pt-BR"
# Response: {"message": "Usuário não encontrado."}

# OR with header
curl -X GET "https://localhost:7001/api/user/999" -H "Accept-Language: pt-BR"
# Response: {"message": "Usuário não encontrado."}
```

### Spanish
```bash
curl -X GET "https://localhost:7001/api/user/999?culture=es-ES"
# Response: {"message": "Usuario no encontrado."}

# OR with header
curl -X GET "https://localhost:7001/api/user/999" -H "Accept-Language: es-ES"
# Response: {"message": "Usuario no encontrado."}
```

## Architecture Benefits

- **Centralized Resources**: All messages are in one place (`SharedResource` files)
- **Extensible**: Easy to add new languages and messages
- **Clean Controllers**: No hardcoded strings in business logic
- **Standard ASP.NET Core**: Uses Microsoft's built-in localization features
- **Flexible Selection**: Supports both query string and header-based culture selection
- **Fallback**: Always falls back to English if a translation is missing
