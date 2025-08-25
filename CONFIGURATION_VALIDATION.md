# Validação e Health Checks de Configurações

Este documento explica como funciona a validação de configurações e health checks implementados na aplicação.

## Visão Geral

A aplicação agora valida automaticamente todas as configurações essenciais na inicialização e expõe health checks para monitoramento contínuo.

### Ordem de Prioridade das Configurações

1. **appsettings.json** - Configurações base
2. **appsettings.{Environment}.json** - Configurações específicas do ambiente
3. **User Secrets** - Apenas em Development
4. **Environment Variables** - Variáveis de ambiente
5. **Docker Secrets** - Secrets montados em containers

## Classes de Configuração

### JwtSettings

```csharp
public class JwtSettings
{
    [Required, MinLength(32)]
    public string SecretKey { get; set; }
    
    [Required, Url]
    public string Issuer { get; set; }
    
    [Required, Url]
    public string Audience { get; set; }
    
    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; set; } = 60;
}
```

### DatabaseSettings

```csharp
public class DatabaseSettings
{
    [Required, MinLength(10)]
    public string ConnectionString { get; set; }
    
    [Range(1, 300)]
    public int CommandTimeout { get; set; } = 30;
    
    public bool AutoMigrate { get; set; } = false;
    public bool SeedData { get; set; } = false;
    public string LogLevel { get; set; } = "Warning";
    public bool EnableSensitiveDataLogging { get; set; } = false;
}
```

### CorsSettings

```csharp
public class CorsSettings
{
    [Required, MinLength(1)]
    public string[] AllowedOrigins { get; set; }
    
    public string[] AllowedMethods { get; set; }
    public string[] AllowedHeaders { get; set; }
    public bool AllowCredentials { get; set; } = true;
    public int PreflightMaxAge { get; set; } = 3600;
    public string[] ExposedHeaders { get; set; }
}
```

## Uso do Options Pattern

### IOptions<T> vs IOptionsSnapshot<T>

```csharp
// IOptions<T> - Singleton, valor não muda durante execução
private readonly IOptions<JwtSettings> _jwtSettings;

// IOptionsSnapshot<T> - Scoped, pode mudar entre requests
private readonly IOptionsSnapshot<DatabaseSettings> _dbSettings;

public MyService(IOptions<JwtSettings> jwt, IOptionsSnapshot<DatabaseSettings> db)
{
    _jwtSettings = jwt;
    _dbSettings = db;
}

public void MyMethod()
{
    var jwtConfig = _jwtSettings.Value;
    var dbConfig = _dbSettings.Value; // Pode ter valor atualizado
}
```

### Configuração no Program.cs

```csharp
// Configurar e validar automaticamente
builder.Services.AddAndValidateConfigurations(builder.Configuration);

// Agora pode injetar via construtor
public class MyController : ControllerBase
{
    private readonly IOptions<JwtSettings> _jwt;
    
    public MyController(IOptions<JwtSettings> jwt)
    {
        _jwt = jwt;
    }
}
```

## Health Checks

### Endpoints Disponíveis

- **GET /health** - Health check geral da aplicação
- **GET /health/config** - Health check específico para configurações

### ConfigHealthCheck

Verifica automaticamente:

- ✅ **JwtSettings**: SecretKey (mín. 32 chars), Issuer (URL válida), Audience (URL válida)
- ✅ **DatabaseSettings**: ConnectionString (mín. 10 chars), CommandTimeout válido
- ✅ **CorsSettings**: AllowedOrigins não vazio, configuração segura

### Exemplo de Resposta (/health/config)

```json
{
  "status": "Healthy",
  "checks": {
    "config": {
      "status": "Healthy",
      "description": "Todas as configurações essenciais estão válidas",
      "data": {
        "jwt_config": {
          "SecretKey": "***CONFIGURED***",
          "Issuer": "https://localhost:5001",
          "Audience": "https://localhost:5001",
          "ExpiryMinutes": 60
        },
        "jwt_valid": true,
        "database_config": {
          "ConnectionString": "***CONFIGURED***",
          "CommandTimeout": 30,
          "AutoMigrate": false
        },
        "database_valid": true,
        "cors_valid": true
      }
    }
  },
  "duration": "00:00:00.0234567"
}
```

## Validação na Inicialização

### Como Funciona

A aplicação valida todas as configurações obrigatórias durante o startup:

```csharp
// No Program.cs
builder.Services.AddAndValidateConfigurations(builder.Configuration);
```

### Exemplo de Falha

Se uma configuração obrigatória estiver ausente:

```
Unhandled exception. System.InvalidOperationException: 
Configuração inválida na seção 'JwtSettings':
  - JWT SecretKey é obrigatório
  - JWT Issuer deve ser uma URL válida
   at CoreApiBase.Extensions.IServiceCollectionExtensions.AddAndValidateConfigurations()
```

## Adicionando Novas Configurações

### 1. Criar Classe de Configuração

```csharp
public class MinhaNovaConfig
{
    public const string SectionName = "MinhaNovaConfig";
    
    [Required]
    public string ValorObrigatorio { get; set; } = string.Empty;
    
    [Range(1, 100)]
    public int NumeroValido { get; set; } = 10;
}
```

### 2. Adicionar no appsettings.json

```json
{
  "MinhaNovaConfig": {
    "ValorObrigatorio": "algum-valor",
    "NumeroValido": 25
  }
}
```

### 3. Registrar no Program.cs

```csharp
// Na extensão AddAndValidateConfigurations
var minhaSection = configuration.GetSection(MinhaNovaConfig.SectionName);
services.Configure<MinhaNovaConfig>(minhaSection);

var minhaConfig = minhaSection.Get<MinhaNovaConfig>() ?? new MinhaNovaConfig();
minhaConfig.ValidateConfiguration(MinhaNovaConfig.SectionName);
```

### 4. Adicionar no Health Check

```csharp
// No ConfigHealthCheck.cs
private readonly IOptions<MinhaNovaConfig> _minhaConfig;

// No construtor
public ConfigHealthCheck(..., IOptions<MinhaNovaConfig> minhaConfig)
{
    _minhaConfig = minhaConfig;
}

// No método CheckHealthAsync
private void CheckMinhaConfig(Dictionary<string, object> healthData, List<string> errors)
{
    var config = _minhaConfig.Value;
    var configErrors = config.GetValidationErrors();
    
    if (configErrors.Any())
    {
        errors.AddRange(configErrors.Select(e => $"MinhaConfig: {e}"));
    }
    
    healthData["minha_config"] = config.GetConfigurationSummary();
    healthData["minha_valid"] = !configErrors.Any();
}
```

### 5. Usar na Aplicação

```csharp
public class MeuController : ControllerBase
{
    private readonly IOptions<MinhaNovaConfig> _config;
    
    public MeuController(IOptions<MinhaNovaConfig> config)
    {
        _config = config;
    }
    
    [HttpGet]
    public ActionResult Get()
    {
        var valor = _config.Value.ValorObrigatorio;
        return Ok(valor);
    }
}
```

## Extensibilidade para Kubernetes

A estrutura atual permite facilmente adicionar suporte a Kubernetes Secrets:

```csharp
// Futuro: KubernetesSecretsConfigurationSource
builder.Configuration.AddKubernetesSecrets(secrets =>
{
    secrets.WithSecretsPath("/etc/secrets") // Kubernetes monta aqui
           .AddSecret("jwt-secret", "JwtSettings:SecretKey")
           .AddSecret("db-password", "DatabaseSettings:ConnectionString");
});
```

## Comandos Úteis

### Testar Health Checks

```bash
# Health check geral
curl http://localhost:5000/health

# Health check de configurações
curl http://localhost:5000/health/config

# Ver configurações (desenvolvimento)
curl http://localhost:5000/api/configdemo/all
```

### Definir User Secrets (Development)

```bash
dotnet user-secrets set "JwtSettings:SecretKey" "meu-super-secret-key-de-desenvolvimento"
dotnet user-secrets set "DatabaseSettings:ConnectionString" "Server=dev;Database=DevDb;..."
```

### Environment Variables (Produção)

```bash
export JwtSettings__SecretKey="production-secret-key"
export DatabaseSettings__ConnectionString="production-connection-string"
```

## Logs de Configuração

A aplicação registra automaticamente:

- ✅ Configurações carregadas com sucesso
- ❌ Erros de validação durante startup
- 🔍 Resultados de health checks
- ⚠️ Configurações inseguras detectadas

### Exemplo de Log

```
[Information] Configurações validadas com sucesso: JwtSettings, DatabaseSettings, CorsSettings
[Warning] CORS: Configuração insegura detectada - AllowCredentials=true com AllowedOrigins=*
[Error] Configuração inválida na seção 'JwtSettings': JWT SecretKey é obrigatório
```
