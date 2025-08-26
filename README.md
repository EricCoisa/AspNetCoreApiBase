# ASP.NET Core API Base - StarterPack

🚀 **Projeto base para criação rápida de APIs ASP.NET Core com Entity Framework, AutoMapper e SQLite**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0.8-green.svg)](https://docs.microsoft.com/ef/)
[![SQLite](https://img.shields.io/badge/SQLite-Database-lightgrey.svg)](https://www.sqlite.org/)
[![AutoMapper](https://img.shields.io/badge/AutoMapper-12.0.1-orange.svg)](https://automapper.org/)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue.svg)](https://www.docker.com/)

## 📋 Sobre o Projeto

Este é um projeto starter completo para APIs ASP.NET Core com todas as configurações de segurança, Docker e desenvolvimento já prontas. O sistema inclui configuração automática de secrets, health checks, autenticação JWT e suporte completo para desenvolvimento local e produção em Docker.

## 🚀 **INÍCIO RÁPIDO**

### 🆕 **Primeiro uso (após clonar o repositório):**

```bash
# 1. Configure o ambiente (apenas uma vez)
setup-configuration.bat Development     # Windows
./setup-configuration.sh Development    # Linux/Mac

# 2. Execute a aplicação  
cd CoreApiBase
dotnet run
```

**Pronto!** O banco será criado automaticamente na primeira execução.

**Acesse:**
- **🌐 API:** http://localhost:5099
- **📖 Swagger:** http://localhost:5099/swagger
- **🔍 Health:** http://localhost:5099/health

---
### 🐳 **Para Docker:**

```bash
# Configure para Docker
setup-configuration.bat Docker    # Windows  
./setup-configuration.sh Docker   # Linux/Mac

# Escolha o tipo de banco:
# 1. Volume isolado (padrão) - bancos separados
# 2. Banco compartilhado - mesmo banco local e Docker

# Execute
docker-compose up -d

# Acesse: http://localhost:8080/swagger
```

**💡 Configuração de Banco:** Veja [DOCKER_DATABASE_CONFIG.md](./DOCKER_DATABASE_CONFIG.md) para detalhes sobre bancos isolados vs compartilhados.

---

## 💻 **DESENVOLVIMENTO - Visual Studio & VS Code**

### 🟦 **Visual Studio 2022**

#### **Opção 1: IIS Express (Desenvolvimento)**
1. **Configure o projeto:**
   ```bash
   setup-configuration.bat development
   ```

2. **No Visual Studio:**
   - Abra `CoreApiBase.sln`
   - Selecione profile **"http"** ou **"https"**
   - Pressione `F5` ou clique em "Iniciar"

3. **URLs disponíveis:**
   - **API:** `http://localhost:5099` ou `https://localhost:7053`
   - **Swagger:** `http://localhost:5099/swagger`

#### **Opção 2: Docker no Visual Studio**
1. **⚠️ IMPORTANTE: Configure para Docker primeiro:**
   ```bash
   setup-configuration.bat docker
   ```

2. **No Visual Studio:**
   - Selecione profile **"Container (Dockerfile)"** para desenvolvimento
   - OU selecione **"Docker (Production)"** para produção
   - Pressione `F5` para debug em container

3. **URLs Docker:**
   - **API:** `http://localhost:8080`
   - **Swagger:** `http://localhost:8080/swagger`

**💡 Nota:** O script configura automaticamente as variáveis de ambiente no `launchSettings.json` para o Visual Studio usar.

**� Segurança:** O arquivo `launchSettings.template.json` (sem secrets) é versionado no Git, enquanto o arquivo real `launchSettings.json` é ignorado pelo `.gitignore`. Todos os scripts de configuração (`Development`, `Docker`, `Production`) criam automaticamente o arquivo real a partir do template, mantendo as credenciais seguras.

**�📖 Guia Completo:** Veja [VISUAL_STUDIO_DOCKER.md](./VISUAL_STUDIO_DOCKER.md) para detalhes completos e troubleshooting.

#### **Opção 3: Release Mode (Produção)**
1. **Configure para Release:**
   ```bash
   setup-configuration.bat release
   ```

2. **No Visual Studio:**
   - Selecione profile **"http (Release)"** ou **"https (Release)"**
   - Compile em modo Release
   - Execute `Ctrl+F5`

### 🟩 **VS Code**

#### **Desenvolvimento Local**
1. **Configure:**
   ```bash
   # Windows
   setup-configuration.bat development
   
   # Linux/Mac
   ./setup-configuration.sh development
   ```

2. **Execute no terminal integrado:**
   ```bash
   dotnet run --project CoreApiBase
   ```

3. **Para debug:**
   - Pressione `F5`
   - Ou use `Ctrl+Shift+P` → "Debug: Start Debugging"

#### **Docker no VS Code**
1. **Configure para Docker:**
   ```bash
   # Windows
   setup-configuration.bat docker
   
   # Linux/Mac
   ./setup-configuration.sh docker
   ```

2. **Execute Docker:**
   ```bash
   docker-compose up --build
   ```

3. **Para debug com Docker:**
   - Instale extensão "Docker" 
   - Use `Ctrl+Shift+P` → "Docker: Compose Up"

---

## 🐳 **DOCKER - Produção e Containers**

### **Docker Compose (Recomendado)**

1. **Configuração inicial:**
   ```bash
   # Windows
   setup-configuration.bat docker
   
   # Linux/Mac
   ./setup-configuration.sh docker
   ```

2. **Execute em background:**
   ```bash
   docker-compose up -d --build
   ```

3. **Monitore logs:**
   ```bash
   docker-compose logs -f
   ```

4. **Pare os containers:**
   ```bash
   docker-compose down
   ```

### **Docker Manual**

1. **Build da imagem:**
   ```bash
   docker build -t coreapi:latest -f CoreApiBase/Dockerfile .
   ```

2. **Execute o container:**
   ```bash
   docker run -d -p 8080:8080 \
     --name coreapi-container \
     -v ${PWD}/secrets:/run/secrets:ro \
     coreapi:latest
   ```

### **URLs Docker:**
- **API:** `http://localhost:8080`
- **Swagger:** `http://localhost:8080/swagger`
- **Health:** `http://localhost:8080/health`

---

## 🌐 **IIS - Deploy em Servidor Windows**

### **IIS Local (Desenvolvimento)**

1. **Publique o projeto:**
   ```bash
   dotnet publish CoreApiBase -c Release -o ./publish
   ```

2. **Configure no IIS Manager:**
   - Crie um novo Site
   - Aponte para a pasta `./publish`
   - Configure Application Pool para `.NET Core`
   - Defina porta (ex: 8080)

### **IIS Express (Automático)**

O projeto já vem configurado com profiles para IIS Express:

```json
"iisSettings": {
  "windowsAuthentication": false,
  "anonymousAuthentication": true,
  "iisExpress": {
    "applicationUrl": "http://localhost:5099",
    "sslPort": 7053
  }
}
```

### **Windows Server (Produção)**

1. **Configure segurança:**
   ```bash
   setup-configuration.bat production
   ```

2. **Publique para produção:**
   ```bash
   dotnet publish CoreApiBase -c Release -o C:\inetpub\wwwroot\coreapi
   ```

3. **Configure IIS:**
   - Instale [.NET Core Hosting Bundle](https://dotnet.microsoft.com/permalink/dotnetcore-current-windows-runtime-bundle-installer)
   - Crie Application Pool (.NET Core, Integrated)
   - Configure site apontando para pasta de publicação
   - Configure variáveis de ambiente no IIS

---

## ⚙️ **CONFIGURAÇÕES POR AMBIENTE**

### 🔧 **Scripts Automáticos**

| Script | Ambiente | Descrição |
|--------|----------|-----------|
| `setup-configuration.bat development` | Desenvolvimento | User Secrets + SQLite local |
| `setup-configuration.bat release` | Release/Produção | Environment Variables |
| `setup-configuration.bat docker` | Docker | Docker Secrets |
| `setup-configuration.bat production` | Produção | Input interativo |

### 🏃‍♂️ **Profiles de Execução**

#### **Visual Studio Profiles:**
- **http/https**: IIS Express desenvolvimento
- **http (Release)/https (Release)**: IIS Express produção
- **Container (Dockerfile)**: Debug em Docker
- **Docker Compose**: Orquestração completa

#### **URLs por Profile:**
| Profile | URL API | URL Swagger | Ambiente |
|---------|---------|-------------|----------|
| http | `http://localhost:5099` | `/swagger` | Development |
| https | `https://localhost:7053` | `/swagger` | Development |
| http (Release) | `http://localhost:5099` | `/swagger` | Release |
| Container | `http://localhost:8080` | `/swagger` | Docker |
| Docker Compose | `http://localhost:8080` | `/swagger` | Docker |

---

## 🔒 **SISTEMA DE CONFIGURAÇÃO SEGURA**

### **Hierarquia de Configuração (5 Níveis)**

1. **appsettings.json** (base)
2. **appsettings.{Environment}.json** (ambiente)
3. **User Secrets** (desenvolvimento local)
4. **Environment Variables** (sistema/container)
5. **Docker Secrets** (produção) - **Maior prioridade**

### **Configurações Obrigatórias**

```bash
JWT_SECRET_KEY=chave-secreta-jwt
DATABASE_CONNECTION_STRING=Data Source=appdb.sqlite
CORS_ALLOWED_ORIGINS=http://localhost:3000
```

### **Validação Automática**

O sistema detecta automaticamente configurações faltando e:
- **Desktop**: Abre página HTML com instruções
- **Container**: Mostra instruções específicas no console
- **Desenvolvimento**: Sugestões de User Secrets
- **Produção**: Instruções para variáveis de ambiente

---

## 🧪 **TESTES E HEALTH CHECKS**

### **Endpoints de Saúde**
| Endpoint | Descrição | Swagger |
|----------|-----------|---------|
| `/health` | Status geral | ✅ |
| `/health/ready` | Prontidão | ✅ |
| `/health/live` | Vitalidade | ✅ |

### **Testes Rápidos**
```bash
# Health check
curl http://localhost:5099/health

# Swagger API
curl http://localhost:5099/swagger/v1/swagger.json

# Registro de usuário
curl -X POST http://localhost:5099/Authentication/register \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","email":"test@test.com","password":"123456"}'
```

---

## 🔧 **TROUBLESHOOTING**

### **Problemas Comuns**

#### **"Configurações obrigatórias faltando"**
```bash
# Execute o script de configuração
setup-configuration.bat development
```

#### **"Erro de conexão com banco"**
```bash
# Recrie o banco
dotnet ef database drop --project CoreApiBase --force
dotnet ef database update --project CoreApiBase
```

#### **"Porta já está em uso"**
- Mude a porta em `appsettings.json` → `Kestrel:Endpoints:Http:Url`
- Ou mate o processo: `netstat -ano | findstr :5099`

#### **"Docker não inicia"**
```bash
# Limpe tudo e reconfigure
cleanup-secrets.bat
setup-configuration.bat docker
docker-compose up --build
```

### **Reset Completo**
```bash
# Limpa todas as configurações
cleanup-secrets.bat        # Windows
./cleanup-secrets.sh       # Linux/Mac

# Reconfigure do zero
setup-configuration.bat development
```

---

## 🏗️ **ARQUITETURA DO PROJETO**

O projeto segue uma arquitetura em camadas bem definida:

```
CoreApiBase/          # 🎯 Camada de Apresentação (API)
├── Controllers/      # Controladores da API
│   ├── AuthenticationController.cs  # Endpoints de autenticação
│   └── UserController.cs           # CRUD de usuários (Admin)
├── Application/      # DTOs e modelos de aplicação
│   ├── DTOs/        # Objetos de transferência de dados
│   └── Profiles/    # Perfis do AutoMapper
├── Extensions/       # Métodos de extensão para DI
├── Configurations/   # Configurações do projeto
│   ├── AppSettings.cs              # Configurações da aplicação
│   ├── AutoMapperConfig.cs         # Configuração do AutoMapper
│   ├── JwtSettings.cs              # Configurações JWT
│   └── ConfigurationSetupHelper.cs # Sistema de validação
├── Middlewares/      # Middlewares customizados
│   ├── AccessMiddleware.cs         # Controle de acesso
│   ├── AuthLoggingMiddleware.cs    # Log de autenticação
│   └── SecurityStampValidationMiddleware.cs # Validação de security stamp
└── Utils/           # Utilitários
    └── TokenDataHandler.cs         # Manipulação de tokens

CoreDomainBase/       # 🏢 Camada de Domínio e Dados
├── Entities/         # Entidades do domínio
│   └── User.cs      # Entidade de usuário
├── Services/         # Serviços de negócio
│   └── UserService.cs              # Lógica de negócio de usuários
├── Repositories/     # Repositórios de dados
│   ├── RepositoriesBase.cs         # Repository genérico
│   └── UserRepositories.cs         # Repository específico de usuários
├── Interfaces/       # Contratos e interfaces
│   ├── Repositories/ # Interfaces de repositórios
│   └── Services/    # Interfaces de serviços
├── Enums/           # Enumerações
│   ├── Policy.cs    # Políticas de autorização
│   └── Roles.cs     # Roles do sistema
└── Data/            # Contexto do banco e configurações EF
    ├── AppDbContext.cs             # Contexto principal
    └── Configurations/             # Configurações das entidades

Tests/               # 🧪 Testes
├── UnitTests/       # Testes unitários
└── IntegrationTests/ # Testes de integração
```

## ⚡ **CARACTERÍSTICAS PRINCIPAIS**

- ✅ **ASP.NET Core 8.0** - Framework moderno e performático
- ✅ **Entity Framework Core** - ORM com SQLite pré-configurado
- ✅ **AutoMapper** - Mapeamento automático entre entidades e DTOs
- ✅ **Swagger/OpenAPI** - Documentação automática da API
- ✅ **Injeção de Dependência** - Configurada e pronta para uso
- ✅ **Padrão Repository** - Implementação genérica de repositórios
- ✅ **CRUD Completo** - Exemplo funcional com entidade User
- ✅ **Migrations** - Controle de versão do banco de dados
- ✅ **Estrutura Limpa** - Separação clara de responsabilidades
- ✅ **Autenticação JWT** - Sistema seguro de autenticação
- ✅ **Autorização por Roles** - Controle granular de acesso
- ✅ **Health Checks** - Monitoramento da aplicação
- ✅ **Docker Support** - Containerização pronta para produção
- ✅ **Configuração Segura** - Sistema automático de secrets
- ✅ **Security Stamp** - Invalidação de tokens por usuário
- ✅ **CORS Configurável** - Suporte para frontends SPA
- ✅ **Middleware Pipeline** - Logging e validação automáticos

---

## 📊 Health Checks

A aplicação inclui endpoints de saúde integrados ao Swagger:

- **GET** `/health` - Status geral da aplicação
- **GET** `/health/ready` - Verificação de prontidão
- **GET** `/health/live` - Verificação de vitalidade

### Validações Incluídas
- ✅ Configurações obrigatórias
- ✅ Conectividade com banco de dados  
- ✅ Validação de chaves JWT
- ✅ Status dos serviços essenciais

## 🎯 **API ENDPOINTS DISPONÍVEIS**

### **🔐 Authentication API**
| Método | Endpoint | Descrição | Auth Required |
|--------|----------|-----------|---------------|
| POST | `/Authentication/register` | Registra novo usuário | ❌ |
| POST | `/Authentication/login` | Autentica usuário | ❌ |
| GET | `/Authentication/profile` | Perfil do usuário logado | ✅ JWT |
| POST | `/Authentication/refresh-token` | Atualiza token JWT | ✅ JWT |
| POST | `/Authentication/revoke-token` | Revoga token de usuário | ✅ JWT |

### **👥 Users API (Admin)**
| Método | Endpoint | Descrição | Auth Required |
|--------|----------|-----------|---------------|
| GET | `/User` | Lista todos os usuários | ✅ Admin |
| GET | `/User/{id}` | Busca usuário por ID | ✅ Admin |
| POST | `/User` | Cria novo usuário | ✅ Admin |
| PUT | `/User/{id}` | Atualiza usuário | ✅ Admin |
| DELETE | `/User/{id}` | Remove usuário | ✅ Admin |

### **🏥 Health Checks**
| Método | Endpoint | Descrição | Swagger |
|--------|----------|-----------|---------|
| GET | `/health` | Status geral | ✅ |
| GET | `/health/ready` | Prontidão da aplicação | ✅ |
| GET | `/health/live` | Vitalidade da aplicação | ✅ |

### **📋 Exemplo de Payloads**

#### **Registro de Usuário**
```json
POST /Authentication/register
{
  "name": "João Silva",
  "email": "joao@email.com",
  "password": "123456"
}
```

#### **Login**
```json
POST /Authentication/login
{
  "email": "joao@email.com",
  "password": "123456"
}
```

#### **Resposta de Login**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "expiration": "2025-08-26T15:30:00Z",
  "user": {
    "id": 1,
    "name": "João Silva",
    "email": "joao@email.com",
    "role": "User"
  }
}
```

### **🌐 URLs de Acesso**

#### **Desenvolvimento (IIS Express)**
- **API:** `http://localhost:5099`
- **API HTTPS:** `https://localhost:7053`
- **Swagger:** `http://localhost:5099/swagger`
- **Health Check:** `http://localhost:5099/health`

#### **Docker**
- **API:** `http://localhost:8080`
- **Swagger:** `http://localhost:8080/swagger`
- **Health Check:** `http://localhost:8080/health`

---

## ⚙️ **CONFIGURAÇÃO AVANÇADA**

### **🔒 Sistema de Configuração Segura**

O projeto implementa um sistema de configuração com **5 níveis de prioridade**:

1. **appsettings.json** (base)
2. **appsettings.{Environment}.json** (ambiente específico)
3. **User Secrets** (desenvolvimento local)
4. **Environment Variables** (sistema/container)
5. **Docker Secrets** (produção Docker) - **Maior prioridade**

#### **Configurações Disponíveis**
- `JWT_SECRET_KEY`: Chave secreta para tokens JWT
- `DATABASE_CONNECTION_STRING`: String de conexão do banco
- `CORS_ALLOWED_ORIGINS`: Origens permitidas para CORS

#### **Scripts de Configuração**

O projeto possui **scripts únicos** que fazem toda a configuração automaticamente:

- `setup-configuration.bat` (Windows)  
- `setup-configuration.sh` (Linux/Mac)

**Funcionalidades dos scripts:**
- ✅ Geração segura de chaves criptográficas
- ✅ Configuração de User Secrets para desenvolvimento
- ✅ Preparação de Docker Secrets para produção
- ✅ Validação de configurações
- ✅ Detecção automática de ambiente

#### **Script de Limpeza**

Para testar do zero ou limpar configurações antigas:

- `cleanup-secrets.bat` (Windows)
- `cleanup-secrets.sh` (Linux/Mac)

**O que o script remove:**
- 🧹 Todos os User Secrets
- 🧹 Pasta secrets/ e arquivos de configuração
- 🧹 Containers e volumes Docker
- 🧹 Bancos de dados SQLite locais

### **🚀 Adicionando Novas Entidades**

1. **Crie a entidade** em `CoreDomainBase/Entities/`
   ```csharp
   public class MinhaEntidade
   {
       public int Id { get; set; }
       public string Nome { get; set; }
   }
   ```

2. **Crie o DTO** em `CoreApiBase/Application/DTOs/`
   ```csharp
   public class MinhaEntidadeDto
   {
       public int Id { get; set; }
       public string Nome { get; set; }
   }
   ```

3. **Configure o mapeamento** em `CoreDomainBase/Data/Configurations/`
   ```csharp
   public class MinhaEntidadeConfiguration : IEntityTypeConfiguration<MinhaEntidade>
   {
       public void Configure(EntityTypeBuilder<MinhaEntidade> builder)
       {
           builder.HasKey(x => x.Id);
           builder.Property(x => x.Nome).IsRequired().HasMaxLength(100);
       }
   }
   ```

4. **Adicione ao DbContext** em `CoreDomainBase/Data/AppDbContext.cs`
   ```csharp
   public DbSet<MinhaEntidade> MinhasEntidades { get; set; }
   ```

5. **Execute a migration**:
   ```bash
   dotnet ef migrations add AddMinhaEntidade --project CoreApiBase
   dotnet ef database update --project CoreApiBase
   ```

### **🎛 Customizando Connection String**

#### **Por Ambiente**

**appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=appdb_dev.sqlite"
  }
}
```

**appsettings.Production.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/app/data/production.db"
  }
}
```

#### **Com User Secrets (Desenvolvimento)**
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=meudb.sqlite" --project CoreApiBase
```

#### **Com Docker Secrets (Produção)**
```bash
echo "Data Source=/app/data/production.db" | docker secret create db_connection_string -
```

#### **Com Variáveis de Ambiente**
```bash
# Windows
set DATABASE_CONNECTION_STRING=Data Source=mydb.sqlite

# Linux/Mac
export DATABASE_CONNECTION_STRING="Data Source=mydb.sqlite"
```

---

## 🗄️ **CONFIGURAÇÃO DE BANCO DE DADOS**

### **🔧 Banco de Dados Padrão (SQLite com Auto-Migration)**

Por padrão, a aplicação usa SQLite com migrações automáticas:

```bash
# Configuração padrão (com auto-migration)
setup-configuration.bat Development

# O banco será criado automaticamente em: ./appdb.sqlite
```

### **🎛️ Usando Seu Próprio Banco de Dados**

Se você já tem um banco de dados e quer desabilitar as migrações automáticas:

```bash
# Configure sem migrações automáticas
setup-configuration.bat Development --no-migrate
```

**O que acontece com `--no-migrate`:**
- ✅ AutoMigrate é configurado como `false`
- ✅ A aplicação não tentará aplicar migrações automaticamente
- ✅ Você mantém controle total sobre o schema do banco
- ✅ Pode usar SQL Server, PostgreSQL, MySQL, etc.

### **📖 Documentação Completa**

Para configuração detalhada de bancos customizados, veja:
**[CUSTOM_DATABASE.md](./CUSTOM_DATABASE.md)** - Guia completo com exemplos para SQL Server, PostgreSQL, MySQL e mais.

---

## 🧪 **TESTES E MONITORAMENTO**

### **🏥 Health Checks Integrados**

A aplicação inclui endpoints de saúde integrados ao Swagger:

#### **Endpoints Disponíveis**
| Endpoint | Descrição | Status Codes | Swagger |
|----------|-----------|--------------|---------|
| `/health` | Status geral da aplicação | 200, 503 | ✅ |
| `/health/ready` | Verificação de prontidão | 200, 503 | ✅ |
| `/health/live` | Verificação de vitalidade | 200, 503 | ✅ |

#### **Validações Incluídas**
- ✅ Configurações obrigatórias (JWT, DB, CORS)
- ✅ Conectividade com banco de dados SQLite
- ✅ Validação de chaves JWT válidas
- ✅ Status dos serviços essenciais
- ✅ Verificação de memória e recursos

#### **Exemplos de Teste**
```bash
# Health check básico
curl http://localhost:5099/health

# Health check detalhado
curl http://localhost:5099/health?detailed=true

# Teste de prontidão
curl http://localhost:8080/health/ready

# Health check em Docker
curl http://localhost:8080/health
```

### **🧪 Executando Testes**

#### **Testes Unitários**
```bash
# Executar todos os testes
dotnet test

# Executar testes específicos
dotnet test --filter "Category=Unit"

# Com coverage
dotnet test --collect:"XPlat Code Coverage"
```

#### **Testes de Integração**
```bash
# Testes de integração
dotnet test --filter "Category=Integration"

# Testes da API
dotnet test --filter "TestCategory=API"
```

### **🔍 Testes Manuais da API**

#### **Registro e Login**
```bash
# 1. Registrar usuário
curl -X POST http://localhost:5099/Authentication/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test User",
    "email": "test@example.com",
    "password": "123456"
  }'

# 2. Fazer login
curl -X POST http://localhost:5099/Authentication/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "123456"
  }'

# 3. Usar token nas requisições seguintes
curl -X GET http://localhost:5099/Authentication/profile \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

#### **CRUD de Usuários (Admin)**
```bash
# Listar usuários (requer token de Admin)
curl -X GET http://localhost:5099/User \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN"

# Buscar usuário específico
curl -X GET http://localhost:5099/User/1 \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN"
```

### **📊 Monitoramento de Performance**

#### **Métricas Disponíveis**
- ⏱️ **Response Time**: Tempo de resposta das requisições
- 🔢 **Request Count**: Número de requisições por endpoint
- 💾 **Memory Usage**: Uso de memória da aplicação
- 🗄️ **Database Connections**: Conexões ativas com o banco

#### **Logs Estruturados**
O projeto utiliza logging estruturado com:
- **Serilog** para logs avançados
- **Application Insights** (configurável)
- **Console Logging** para desenvolvimento
- **File Logging** para produção

```bash
# Verificar logs em tempo real (Docker)
docker-compose logs -f coreapi

# Logs do Visual Studio
# Visualizar na janela "Output" → "Debug"
```

---

## 📦 **TECNOLOGIAS E PACOTES**

### **🔧 Pacotes Principais**

| Pacote | Versão | Propósito | Categoria |
|--------|--------|-----------|-----------|
| **Microsoft.EntityFrameworkCore.Sqlite** | 9.0.8 | Provider SQLite para EF Core | 🗄️ Database |
| **Microsoft.EntityFrameworkCore.Design** | 9.0.8 | Ferramentas de design do EF | 🗄️ Database |
| **AutoMapper** | 12.0.1 | Mapeamento objeto-objeto | 📦 Mapping |
| **AutoMapper.Extensions.Microsoft.DependencyInjection** | 12.0.1 | Integração AutoMapper com DI | 📦 Mapping |
| **Swashbuckle.AspNetCore** | 6.6.2 | Documentação Swagger | 📋 Documentation |
| **Microsoft.AspNetCore.Authentication.JwtBearer** | 8.0.0 | Autenticação JWT | 🔐 Security |
| **Microsoft.Extensions.Diagnostics.HealthChecks** | 8.0.0 | Health Checks | 🏥 Monitoring |
| **Microsoft.AspNetCore.Identity.EntityFrameworkCore** | 8.0.0 | Sistema de identidade | 🔐 Security |

### **🎯 Frameworks e Tecnologias**

#### **Backend Core**
- **.NET 8.0** - Framework principal
- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core 9.0** - ORM
- **SQLite** - Banco de dados embarcado

#### **Segurança**
- **JWT (JSON Web Tokens)** - Autenticação stateless
- **ASP.NET Core Identity** - Sistema de usuários e roles
- **CORS** - Cross-Origin Resource Sharing
- **Security Stamp** - Invalidação de tokens por usuário

#### **Desenvolvimento**
- **AutoMapper** - Mapeamento automático
- **Swagger/OpenAPI** - Documentação automática
- **User Secrets** - Configuração segura local
- **Health Checks** - Monitoramento da aplicação

#### **DevOps e Deploy**
- **Docker** - Containerização
- **Docker Compose** - Orquestração
- **Docker Secrets** - Configuração segura em produção
- **Multi-stage Builds** - Otimização de imagens

### **🛠 Ferramentas de Desenvolvimento**

#### **IDEs Suportadas**
- **Visual Studio 2022** (Windows) - Suporte completo
- **Visual Studio Code** (Cross-platform) - Com extensões C#
- **JetBrains Rider** (Cross-platform) - Suporte total

#### **Extensões Recomendadas VS Code**
- **C# Dev Kit** - Suporte oficial Microsoft
- **Docker** - Integração com containers
- **REST Client** - Testes de API
- **GitLens** - Controle de versão avançado

#### **Ferramentas CLI**
- **dotnet CLI** - Comandos .NET
- **dotnet ef** - Entity Framework tools
- **docker** - Containerização
- **docker-compose** - Orquestração

---

## 🔐 **SEGURANÇA**

### **🛡️ JWT Authentication**

#### **Características**
- ✅ **Tokens com expiração configurável** (padrão: 60 minutos)
- ✅ **Refresh tokens** para renovação segura (7 dias)
- ✅ **Security Stamp** para invalidação por usuário
- ✅ **Claims customizados** (ID, Email, Role)
- ✅ **Revogação de tokens** individual

#### **Configuração JWT**
```json
{
  "JwtSettings": {
    "SecretKey": "chave-gerada-automaticamente",
    "Issuer": "CoreApiBase",
    "Audience": "CoreApiBase-Users",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
}
```

### **🔒 Sistema de Configuração Segura**

#### **Hierarquia de Segurança**
1. **Docker Secrets** (Produção) - Mais seguro
2. **Environment Variables** (Sistema)
3. **User Secrets** (Desenvolvimento)
4. **appsettings.{Environment}.json** (Ambiente)
5. **appsettings.json** (Base)

#### **Validação Automática**
- ❌ **Nunca** hardcode secrets em código
- ✅ **Sempre** valida configurações na inicialização
- 🔍 **Detecta** ambiente (Desktop vs Container)
- 📝 **Gera** instruções específicas para cada cenário

### **🌐 CORS (Cross-Origin Resource Sharing)**

#### **Configuração Flexível**
```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "https://meuapp.com"
    ],
    "AllowCredentials": true,
    "AllowedHeaders": ["*"],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"]
  }
}
```

#### **Por Ambiente**
- **Development**: Permite `localhost` em várias portas
- **Production**: Apenas domínios específicos
- **Docker**: Configuração via Docker Secrets

### **👥 Sistema de Roles e Autorização**

#### **Roles Predefinidas**
```csharp
public enum Roles
{
    User = 1,    // Usuário comum
    Admin = 2    // Administrador
}
```

#### **Políticas de Autorização**
- **RequireUser**: Apenas usuários autenticados
- **RequireAdmin**: Apenas administradores
- **RequireOwnerOrAdmin**: Proprietário do recurso ou admin

#### **Middleware de Segurança**
- **AccessMiddleware**: Log de acesso e controle
- **AuthLoggingMiddleware**: Log de tentativas de login
- **SecurityStampValidationMiddleware**: Validação contínua

---

## 🐳 **DOCKER - DEPLOY E PRODUÇÃO**

### **📋 Arquivos Docker**

#### **Dockerfile** (Multi-stage Build Otimizado)
```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore && dotnet build -c Release

# Publish stage  
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CoreApiBase.dll"]
```

#### **docker-compose.yml** (Orquestração Completa)
```yaml
version: '3.8'
services:
  coreapi:
    build:
      context: .
      dockerfile: CoreApiBase/Dockerfile
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    secrets:
      - jwt_key
      - connection_string
    volumes:
      - ./data:/app/data
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

secrets:
  jwt_key:
    file: ./secrets/jwt_key
  connection_string:
    file: ./secrets/connection_string
```

### **🚀 Deploy em Produção**

#### **1. Preparação Local**
```bash
# Configure secrets para produção
setup-configuration.bat production  # Windows
./setup-configuration.sh production # Linux/Mac

# Build e teste local
docker-compose up --build -d
curl http://localhost:8080/health
```

#### **2. Deploy em Servidor**
```bash
# Clone o repositório no servidor
git clone https://github.com/EricCoisa/AspNetCoreApiBase.git
cd AspNetCoreApiBase

# Configure para produção
./setup-configuration.sh production

# Deploy com Docker Swarm
docker swarm init
docker stack deploy -c docker-compose.yml coreapi-stack

# OU com Docker Compose simples
docker-compose up -d --build
```

#### **3. Monitoramento Produção**
```bash
# Verificar status dos containers
docker ps

# Logs em tempo real
docker-compose logs -f

# Health checks
curl http://localhost:8080/health
curl http://localhost:8080/health/ready
```

### **🔧 Configuração para Kubernetes**

#### **kubernetes-deployment.yaml**
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: coreapi-deployment
spec:
  replicas: 3
  selector:
    matchLabels:
      app: coreapi
  template:
    metadata:
      labels:
        app: coreapi
    spec:
      containers:
      - name: coreapi
        image: coreapi:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        volumeMounts:
        - name: secrets-volume
          mountPath: /run/secrets
          readOnly: true
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
          initialDelaySeconds: 30
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 5
      volumes:
      - name: secrets-volume
        secret:
          secretName: coreapi-secrets
```

### **☁️ Deploy em Cloud Providers**

#### **Azure Container Instances**
```bash
# Build e push para Azure Container Registry
az acr build --registry myregistry --image coreapi:latest .

# Deploy no Azure Container Instances
az container create \
  --resource-group myResourceGroup \
  --name coreapi-container \
  --image myregistry.azurecr.io/coreapi:latest \
  --ports 8080 \
  --environment-variables ASPNETCORE_ENVIRONMENT=Production
```

#### **AWS ECS Fargate**
```json
{
  "family": "coreapi-task",
  "containerDefinitions": [
    {
      "name": "coreapi",
      "image": "your-registry/coreapi:latest",
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        }
      ],
      "healthCheck": {
        "command": ["CMD-SHELL", "curl -f http://localhost:8080/health || exit 1"],
        "interval": 30,
        "timeout": 5,
        "retries": 3
      }
    }
  ]
}
```

#### **Google Cloud Run**
```bash
# Build e deploy
gcloud builds submit --tag gcr.io/PROJECT-ID/coreapi .
gcloud run deploy coreapi \
  --image gcr.io/PROJECT-ID/coreapi \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated \
  --port 8080
```

---

## 🚀 **DEPLOY EM IIS (Windows Server)**

### **📋 Pré-requisitos IIS**

#### **1. Instalar .NET Core Hosting Bundle**
```powershell
# Download e instale o bundle mais recente
# https://dotnet.microsoft.com/permalink/dotnetcore-current-windows-runtime-bundle-installer

# Verificar instalação
dotnet --info
```

#### **2. Configurar IIS**
```powershell
# Habilitar IIS no Windows
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole, IIS-WebServer, IIS-CommonHttpFeatures, IIS-HttpRedirection, IIS-NetFxExtensibility45, IIS-ASPNET45

# Instalar módulo ASP.NET Core
# O módulo é instalado automaticamente com o Hosting Bundle
```

### **🚀 Deploy Passo a Passo**

#### **1. Preparação do Projeto**
```bash
# Configure para produção
setup-configuration.bat production

# Publique o projeto
dotnet publish CoreApiBase -c Release -o ./publish --self-contained false
```

#### **2. Configuração no IIS Manager**

1. **Criar Application Pool**
   - Nome: `CoreApiBase`
   - .NET CLR Version: `No Managed Code`
   - Managed Pipeline Mode: `Integrated`

2. **Criar Website**
   - Site name: `CoreApiBase API`
   - Application pool: `CoreApiBase`
   - Physical path: `C:\inetpub\wwwroot\coreapi`
   - Port: `80` ou `443` (HTTPS)

3. **Copiar arquivos**
   ```powershell
   # Copiar arquivos publicados
   Copy-Item -Path "./publish/*" -Destination "C:\inetpub\wwwroot\coreapi" -Recurse -Force
   ```

#### **3. Configuração de Segurança**
```powershell
# Configurar variáveis de ambiente no IIS
# Através do IIS Manager > Configuration Editor > system.webServer/aspNetCore

# OU via web.config
```

#### **4. web.config para IIS**
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\CoreApiBase.dll" 
                  stdoutLogEnabled="false" 
                  stdoutLogFile=".\logs\stdout"
                  hostingModel="InProcess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
          <environmentVariable name="JWT_SECRET_KEY" value="sua-chave-aqui" />
          <environmentVariable name="DATABASE_CONNECTION_STRING" value="Data Source=C:\inetpub\wwwroot\coreapi\data\app.sqlite" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
```

### **🔒 Configuração Segura no IIS**

#### **Usando Configuration Manager**
```csharp
// No IIS, as configurações podem vir de:
// 1. web.config (environmentVariables)
// 2. Application Settings
// 3. Machine-level environment variables
// 4. Azure Key Vault (se configurado)
```

#### **Logs e Monitoramento**
```bash
# Localização dos logs no IIS
C:\inetpub\wwwroot\coreapi\logs\

# Event Viewer
Windows Logs > Application
```

### **🌐 URLs de Acesso IIS**
- **API**: `http://localhost/` ou `http://seu-servidor/`
- **Swagger**: `http://localhost/swagger`
- **Health**: `http://localhost/health`

---

## 🤝 **CONTRIBUIÇÃO E DESENVOLVIMENTO**

### **📋 Como Contribuir**

1. **Fork do projeto**
   ```bash
   # Faça fork do repositório no GitHub
   # Clone seu fork
   git clone https://github.com/SEU-USUARIO/AspNetCoreApiBase.git
   cd AspNetCoreApiBase
   ```

2. **Configure o ambiente**
   ```bash
   # Configure automaticamente
   setup-configuration.bat development  # Windows
   ./setup-configuration.sh development # Linux/Mac
   
   # Teste se está funcionando
   dotnet run --project CoreApiBase
   ```

3. **Crie uma branch para sua feature**
   ```bash
   git checkout -b feature/minha-nova-feature
   ```

4. **Desenvolva e teste**
   ```bash
   # Execute os testes
   dotnet test
   
   # Teste a API
   curl http://localhost:5099/health
   ```

5. **Commit suas mudanças**
   ```bash
   git add .
   git commit -m "feat: adiciona nova funcionalidade X"
   ```

6. **Push e Pull Request**
   ```bash
   git push origin feature/minha-nova-feature
   # Abra um Pull Request no GitHub
   ```

### **� Padrões de Código**

#### **Convenções de Nomenclatura**
- **Classes**: PascalCase (`UserService`, `AuthenticationController`)
- **Métodos**: PascalCase (`GetUserById`, `ValidateToken`)
- **Propriedades**: PascalCase (`Name`, `Email`, `CreatedAt`)
- **Variáveis locais**: camelCase (`userId`, `tokenExpiry`)
- **Constantes**: UPPER_CASE (`JWT_SECRET_KEY`, `DEFAULT_PAGE_SIZE`)

#### **Estrutura de Commits**
```bash
feat: nova funcionalidade
fix: correção de bug
docs: documentação
style: formatação
refactor: refatoração
test: testes
chore: manutenção
```

#### **Padrões de API**
- **REST**: Seguir convenções RESTful
- **HTTP Status Codes**: Usar códigos apropriados (200, 201, 400, 404, 500)
- **Naming**: Endpoints em inglês, kebab-case se necessário
- **Versioning**: Preparado para versionamento (`/v1/`, `/v2/`)

### **🧪 Padrões de Teste**

#### **Estrutura de Testes**
```
Tests/
├── UnitTests/           # Testes unitários isolados
│   ├── Services/        # Testes de serviços
│   ├── Controllers/     # Testes de controllers  
│   └── Repositories/    # Testes de repositórios
└── IntegrationTests/    # Testes de integração
    ├── API/            # Testes de endpoints
    └── Database/       # Testes de banco
```

#### **Nomenclatura de Testes**
```csharp
[Test]
public void MetodoSendoTestado_CondicaoDeEntrada_ComportamentoEsperado()
{
    // Arrange - Given
    // Act - When  
    // Assert - Then
}

// Exemplo:
[Test]
public void GetUserById_ExistingUser_ReturnsUserDto()
{
    // Arrange
    var userId = 1;
    var expectedUser = new User { Id = userId, Name = "Test" };
    
    // Act
    var result = userService.GetUserById(userId);
    
    // Assert
    Assert.That(result.Id, Is.EqualTo(userId));
}
```

### **� Recursos para Desenvolvedores**

#### **Documentação Técnica**
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [AutoMapper Documentation](https://docs.automapper.org/)
- [JWT.IO](https://jwt.io/) - Debug de tokens JWT
- [Swagger Editor](https://editor.swagger.io/) - Editar OpenAPI specs

#### **Ferramentas Úteis**
- **Postman**: Teste de APIs ([Download](https://www.postman.com/downloads/))
- **DB Browser for SQLite**: Visualizar banco SQLite ([Download](https://sqlitebrowser.org/))
- **Docker Desktop**: Desenvolvimento com containers ([Download](https://www.docker.com/products/docker-desktop))
- **Git**: Controle de versão ([Download](https://git-scm.com/))

#### **Extensões Recomendadas**

**Visual Studio:**
- **ReSharper** - Análise de código
- **CodeMaid** - Limpeza de código
- **Productivity Power Tools** - Ferramentas extras

**VS Code:**
- **C# Dev Kit** - Suporte oficial .NET
- **REST Client** - Testes HTTP
- **Docker** - Integração Docker
- **GitLens** - Git avançado
- **Thunder Client** - Cliente REST integrado

---

## 📄 **LICENÇA E INFORMAÇÕES**

### **📜 Licença MIT**

```
MIT License

Copyright (c) 2025 EricCoisa

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

### **🔗 Links Úteis**

#### **Documentação Oficial**
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/) - Framework principal
- [Entity Framework Core](https://docs.microsoft.com/ef/core/) - ORM
- [AutoMapper](https://docs.automapper.org/) - Object mapping
- [Swagger/OpenAPI](https://swagger.io/) - Documentação de API
- [Docker](https://docs.docker.com/) - Containerização
- [JWT](https://jwt.io/) - JSON Web Tokens

#### **Ferramentas e Recursos**
- [.NET 8 Download](https://dotnet.microsoft.com/download/dotnet/8.0) - Runtime e SDK
- [Visual Studio 2022](https://visualstudio.microsoft.com/) - IDE completa
- [VS Code](https://code.visualstudio.com/) - Editor multiplataforma
- [Docker Desktop](https://www.docker.com/products/docker-desktop) - Docker para desenvolvimento
- [Postman](https://www.postman.com/) - Teste de APIs
- [DB Browser for SQLite](https://sqlitebrowser.org/) - Visualizador SQLite

#### **Comunidade e Suporte**
- [GitHub Issues](https://github.com/EricCoisa/AspNetCoreApiBase/issues) - Reportar bugs
- [GitHub Discussions](https://github.com/EricCoisa/AspNetCoreApiBase/discussions) - Discussões
- [Stack Overflow - ASP.NET Core](https://stackoverflow.com/questions/tagged/asp.net-core) - Q&A
- [.NET Community](https://dotnet.microsoft.com/platform/community) - Comunidade oficial

---

## 📖 Documentação Adicional

- **[Configuração de Banco de Dados Docker](DOCKER_DATABASE_CONFIG.md)** - Opções de volume isolado vs compartilhado
- **[Banco de Dados Personalizado](CUSTOM_DATABASE.md)** - Como usar seu próprio banco
- **[Visual Studio + Docker](VISUAL_STUDIO_DOCKER.md)** - Guia completo para desenvolvimento em containers
- **[Troubleshooting Docker](VISUAL_STUDIO_DOCKER_TROUBLESHOOTING.md)** - Soluções para problemas comuns

### **📊 Status do Projeto**

- ✅ **Estável**: Pronto para uso em produção
- 🔄 **Ativo**: Em desenvolvimento contínuo
- 🐛 **Issues**: Reportar bugs via GitHub Issues
- 💡 **Features**: Sugestões via GitHub Discussions
- 📈 **Versão**: v1.0.0
- 🏷️ **Tags**: ASP.NET Core, Docker, JWT, SQLite, Swagger

---

⭐ **Se este projeto foi útil para você, considere dar uma estrela no GitHub!**

**Desenvolvido com ❤️ por [EricCoisa](https://github.com/EricCoisa)**
