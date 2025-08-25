# Docker Secrets Configuration

Este documento explica como configurar e usar Docker Secrets na aplicação.

## Como Funciona

A aplicação segue esta ordem de prioridade para carregar configurações:

1. **appsettings.json** - Configurações base
2. **appsettings.{Environment}.json** - Configurações específicas do ambiente
3. **User Secrets** - Apenas em Development (via `dotnet user-secrets`)
4. **Environment Variables** - Variáveis de ambiente do sistema
5. **Docker Secrets** - Secrets montados em containers Docker

## Configuração de Docker Secrets

### 1. Estrutura de Arquivos

```
project/
├── docker-compose.yml
├── secrets/
│   ├── jwt_secret.txt
│   ├── jwt_issuer.txt
│   ├── jwt_audience.txt
│   ├── db_connection.txt
│   └── cors_origins.txt
└── ...
```

### 2. Docker Compose Configuration

```yaml
version: '3.8'

services:
  app:
    build: .
    secrets:
      - jwt_secret
      - jwt_issuer
      - jwt_audience
      - db_connection
      - cors_origins
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    ports:
      - "8080:80"

secrets:
  jwt_secret:
    file: ./secrets/jwt_secret.txt
  jwt_issuer:
    file: ./secrets/jwt_issuer.txt
  jwt_audience:
    file: ./secrets/jwt_audience.txt
  db_connection:
    file: ./secrets/db_connection.txt
  cors_origins:
    file: ./secrets/cors_origins.txt
```

### 3. Criando Arquivos de Secrets

```bash
# Criar diretório de secrets
mkdir secrets

# Criar secrets individuais
echo "your-super-secret-jwt-key-here" > secrets/jwt_secret.txt
echo "https://yourapp.com" > secrets/jwt_issuer.txt
echo "https://yourapp.com" > secrets/jwt_audience.txt
echo "Server=db;Database=AppDb;User Id=sa;Password=YourPassword;" > secrets/db_connection.txt
echo "https://localhost:3000,https://yourdomain.com" > secrets/cors_origins.txt
```

### 4. Adicionando Novos Secrets

Para adicionar um novo secret:

1. **Adicione o mapeamento no Program.cs:**
```csharp
builder.Configuration.AddDockerSecrets(secrets =>
{
    // Secrets existentes...
    secrets.AddSecret("novo_secret", "MinhaConfig:NovoValor");
});
```

2. **Crie o arquivo de secret:**
```bash
echo "valor-do-secret" > secrets/novo_secret.txt
```

3. **Adicione no docker-compose.yml:**
```yaml
services:
  app:
    secrets:
      - novo_secret  # Adicione aqui

secrets:
  novo_secret:       # E aqui
    file: ./secrets/novo_secret.txt
```

4. **Use na aplicação:**
```csharp
// Via IConfiguration
var valor = builder.Configuration["MinhaConfig:NovoValor"];

// Via Options Pattern
public class MinhaConfig
{
    public string NovoValor { get; set; }
}

// No Program.cs
builder.Services.Configure<MinhaConfig>(builder.Configuration.GetSection("MinhaConfig"));

// No controller/service
public class MeuController : ControllerBase
{
    private readonly MinhaConfig _config;
    
    public MeuController(IOptions<MinhaConfig> config)
    {
        _config = config.Value;
    }
}
```

## Desenvolvimento Local

Em ambiente de desenvolvimento, use User Secrets:

```bash
# Definir secrets para desenvolvimento
dotnet user-secrets set "JwtSettings:SecretKey" "dev-secret-key"
dotnet user-secrets set "JwtSettings:Issuer" "https://localhost:5001"
dotnet user-secrets set "JwtSettings:Audience" "https://localhost:5001"
```

## Segurança

- **Nunca** commit arquivos de secrets no repositório
- Adicione `secrets/` no `.gitignore`
- Use diferentes secrets para cada ambiente
- Rotate secrets regularmente
- Configure permissões adequadas nos arquivos de secrets (600)

## Exemplo de .gitignore

```gitignore
# Docker Secrets
secrets/
*.secret
*.key
```

## Troubleshooting

### Secret não carregado
1. Verifique se o arquivo existe em `/run/secrets/` no container
2. Confirme o mapeamento no Program.cs
3. Verifique a configuração do docker-compose.yml

### Erro de permissão
```bash
# Definir permissões corretas
chmod 600 secrets/*
```

### Verificar se secrets estão carregados
```csharp
// Em um controller de debug
[HttpGet("debug/config")]
public IActionResult GetConfig()
{
    return Ok(new {
        JwtSecret = Configuration["JwtSettings:SecretKey"]?.Substring(0, 5) + "...",
        JwtIssuer = Configuration["JwtSettings:Issuer"],
        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    });
}
```
