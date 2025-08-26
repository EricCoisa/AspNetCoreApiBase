# ASP.NET Core API Base - StarterPack

🚀 **Projeto base para criação rápida de APIs ASP.NET Core com Entity Framework, AutoMapper e SQLite**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0.8-green.svg)](https://docs.microsoft.com/ef/)
[![SQLite](https://img.shields.io/badge/SQLite-Database-lightgrey.svg)](https://www.sqlite.org/)
[![AutoMapper](https://img.shields.io/badge/AutoMapper-12.0.1-orange.svg)](https://automapper.org/)

## 📋 Sobre o Projeto

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas bem definida:

```
CoreApiBase/          # 🎯 Camada de Apresentação (API)
├── Controllers/      # Controladores da API
├── Application/      # DTOs e modelos de aplicação
├── Extensions/       # Métodos de extensão para DI
├── Configurations/   # Configurações do projeto
└── Middlewares/      # Middlewares customizados

CoreDomainBase/       # 🧠 Camada de Domínio e Dados
├── Entities/         # Entidades do domínio
├── Services/         # Serviços de negócio
├── Repositories/     # Repositórios de dados
├── Interfaces/       # Contratos e interfaces
└── Data/            # Contexto do banco e configurações EF
    └── Configurations/ # Configurações das entidades

Tests/               # 🧪 Testes
├── UnitTests/       # Testes unitários
└── IntegrationTests/ # Testes de integração
```

## ✨ Características

- ✅ **ASP.NET Core 8.0** - Framework moderno e performático
- ✅ **Entity Framework Core** - ORM com SQLite pré-configurado
- ✅ **AutoMapper** - Mapeamento automático entre entidades e DTOs
- ✅ **Swagger/OpenAPI** - Documentação automática da API
- ✅ **Injeção de Dependência** - Configurada e pronta para uso
- ✅ **Padrão Repository** - Implementação genérica de repositórios
- ✅ **CRUD Completo** - Exemplo funcional com entidade User
- ✅ **Migrations** - Controle de versão do banco de dados
- ✅ **Estrutura Limpa** - Separação clara de responsabilidades
- ✅ **JWT Authentication** - Sistema de autenticação baseado em tokens
- ✅ **Health Checks** - Monitoramento da saúde da aplicação
- ✅ **Docker Secrets** - Configuração segura para produção
- ✅ **Configuration Validation** - Validação automática de configurações

## 🚀 Como Executar

### Opção 1: Configuração Automática Segura (Recomendado)

**Para desenvolvedores que acabaram de clonar o projeto:**

1. **Clone o repositório**
   ```bash
   git clone https://github.com/EricCoisa/AspNetCoreApiBase.git
   cd AspNetCoreApiBase
   ```

2. **Execute o script de configuração**
   
   **Windows:**
   ```bash
   setup-configuration.bat development
   ```
   
   **Linux/Mac:**
   ```bash
   chmod +x setup-configuration.sh
   ./setup-configuration.sh development
   ```

3. **Execute o projeto**
   ```bash
   dotnet run --project CoreApiBase
   ```

### Opção 2: Docker (Produção)

1. **Configure para Docker**
   ```bash
   # Windows
   setup-configuration.bat docker
   
   # Linux/Mac
   ./setup-configuration.sh docker
   ```

2. **Execute com Docker Compose**
   ```bash
   docker-compose up --build
   ```

### Opção 3: Configuração Manual

Se preferir configurar manualmente:

1. **Copie o arquivo de exemplo**
   ```bash
   copy secrets.env.example secrets.env    # Windows
   cp secrets.env.example secrets.env      # Linux/Mac
   ```

2. **Edite o arquivo `secrets.env`** com suas configurações:
   ```env
   JWT_SECRET_KEY=SuaChaveSecretaAqui
   DATABASE_CONNECTION_STRING=Data Source=appdb.sqlite
   CORS_ALLOWED_ORIGINS=http://localhost:3000,http://localhost:5173
   ```

3. **Configure User Secrets** (opcional para desenvolvimento):
   ```bash
   dotnet user-secrets init --project CoreApiBase
   dotnet user-secrets set "JwtSettings:SecretKey" "SuaChaveSecreta" --project CoreApiBase
   ```

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- [Docker](https://www.docker.com/) (opcional)

## 🔒 Sistema de Configuração Segura

O projeto implementa um sistema de configuração com 5 níveis de prioridade:

1. **appsettings.json** (base)
2. **appsettings.{Environment}.json** (ambiente específico)
3. **User Secrets** (desenvolvimento local)
4. **Environment Variables** (sistema/container)
5. **Docker Secrets** (produção Docker) - **Maior prioridade**

### Configurações Disponíveis

- `JWT_SECRET_KEY`: Chave secreta para tokens JWT
- `DATABASE_CONNECTION_STRING`: String de conexão do banco
- `CORS_ALLOWED_ORIGINS`: Origens permitidas para CORS

### Scripts de Configuração

O projeto agora possui apenas **um script único** que faz toda a configuração automaticamente:

- `setup-configuration.bat` (Windows)  
- `setup-configuration.sh` (Linux/Mac)

Este script único automatiza:
- ✅ Geração segura de chaves criptográficas
- ✅ Configuração de User Secrets para desenvolvimento
- ✅ Preparação de Docker Secrets para produção
- ✅ Validação de configurações

### Script de Limpeza

Para testar do zero ou limpar configurações antigas:

- `cleanup-secrets.bat` (Windows)
- `cleanup-secrets.sh` (Linux/Mac)

Este script remove:
- 🧹 Todos os User Secrets
- 🧹 Pasta secrets/ e arquivos de configuração
- 🧹 Containers e volumes Docker
- 🧹 Bancos de dados SQLite locais

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

## 🎯 Endpoints Disponíveis

### Authentication API
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/Authentication/register` | Registra novo usuário |
| POST | `/Authentication/login` | Autentica usuário |
| GET | `/Authentication/profile` | Perfil do usuário logado |
| POST | `/Authentication/refresh-token` | Atualiza token JWT |
| POST | `/Authentication/revoke-token` | Revoga token de usuário |

### Users API (Admin)
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/User` | Lista todos os usuários |
| GET | `/User/{id}` | Busca usuário por ID |
| POST | `/User` | Cria novo usuário |
| PUT | `/User/{id}` | Atualiza usuário |
| DELETE | `/User/{id}` | Remove usuário |

### Health Checks
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/health` | Status geral |
| GET | `/health/ready` | Prontidão da aplicação |
| GET | `/health/live` | Vitalidade da aplicação |

### Exemplo de Payload
```json
{
  "name": "João Silva",
  "email": "joao@email.com",
  "password": "123456"
}
```

### Acesso à API
- **API:** `http://localhost:5099`
- **Swagger:** `http://localhost:5099/swagger`
- **Health Check:** `http://localhost:5099/health`

## ⚙️ Configuração Avançada

### Adicionando Novas Entidades

1. **Crie a entidade** em `CoreDomainBase/Entities/`
2. **Crie o DTO** em `CoreApiBase/Application/DTOs/`
3. **Configure o mapeamento** em `CoreDomainBase/Data/Configurations/`
4. **Adicione ao DbContext** em `CoreDomainBase/Data/AppDbContext.cs`
5. **Execute a migration**:
   ```bash
   dotnet ef migrations add NomeDaMigration --project CoreApiBase
   dotnet ef database update --project CoreApiBase
   ```

### Customizando Connection String

Para ambientes específicos, edite os arquivos de configuração:

**appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=appdb_dev.sqlite"
  }
}
```

**Docker Secrets (Produção):**
```bash
echo "Data Source=/app/data/production.db" | docker secret create db_connection_string -
```

## 🧪 Testes

Execute os testes do projeto:
```bash
dotnet test
```

### Testes de Health Check
```bash
# Teste direto do endpoint
curl http://localhost:5099/health

# Teste detalhado
curl http://localhost:5099/health?detailed=true
```

## 📦 Pacotes Incluídos

| Pacote | Versão | Propósito |
|--------|--------|-----------|
| Microsoft.EntityFrameworkCore.Sqlite | 9.0.8 | Provider SQLite para EF Core |
| Microsoft.EntityFrameworkCore.Design | 9.0.8 | Ferramentas de design do EF |
| AutoMapper | 12.0.1 | Mapeamento objeto-objeto |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | Integração AutoMapper com DI |
| Swashbuckle.AspNetCore | 6.6.2 | Documentação Swagger |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.0 | Autenticação JWT |
| Microsoft.Extensions.Diagnostics.HealthChecks | 8.0.0 | Health Checks |

## 🔐 Segurança

### JWT Authentication
- Tokens com expiração configurável
- Refresh tokens para renovação segura
- Middleware de validação de security stamp
- Revogação de tokens por usuário

### Configuração Segura
- User Secrets para desenvolvimento
- Docker Secrets para produção
- Variáveis de ambiente como fallback
- Validação automática de configurações obrigatórias

### CORS
Configuração flexível para diferentes ambientes:
```json
{
  "AllowedOrigins": ["http://localhost:3000", "https://meuapp.com"]
}
```

## 🐳 Docker

### Desenvolvimento com Visual Studio
O projeto inclui perfis Docker para o Visual Studio:
- **Container (Dockerfile)**: Debug em container
- **Docker Compose**: Orquestração completa

### Arquivos Docker
- `Dockerfile`: Multi-stage build otimizado
- `docker-compose.yml`: Orquestração com secrets
- `.dockerignore`: Exclusões para build eficiente

## 🚀 Deploy

### Preparação para Produção
1. Execute o script de configuração para produção:
   ```bash
   setup-configuration.bat production  # Windows
   ./setup-configuration.sh production # Linux
   ```

2. Configure os Docker Secrets:
   ```bash
   docker secret create jwt_secret_key jwt_key.txt
   docker secret create db_connection_string db_conn.txt
   ```

3. Deploy com Docker Swarm ou Kubernetes conforme sua infraestrutura.

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 🔗 Links Úteis

- [Documentação ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [AutoMapper](https://docs.automapper.org/)
- [Swagger/OpenAPI](https://swagger.io/)
- [Docker Secrets](https://docs.docker.com/engine/swarm/secrets/)
- [Health Checks](https://docs.microsoft.com/aspnet/core/host-and-deploy/health-checks)

---

⭐ **Se este projeto foi útil para você, considere dar uma estrela!**

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas bem definida:

```
CoreApiBase/          # 🎯 Camada de Apresentação (API)
├── Controllers/      # Controladores da API
├── Application/      # DTOs e modelos de aplicação
├── Extensions/       # Métodos de extensão para DI
├── Configurations/   # Configurações do projeto
└── Middlewares/      # Middlewares customizados

CoreDomainBase/       # 🏢 Camada de Domínio e Dados
├── Entities/         # Entidades do domínio
├── Services/         # Serviços de negócio
├── Repositories/     # Repositórios de dados
├── Interfaces/       # Contratos e interfaces
└── Data/            # Contexto do banco e configurações EF
    └── Configurations/ # Configurações das entidades

Tests/               # 🧪 Testes
├── UnitTests/       # Testes unitários
└── IntegrationTests/ # Testes de integração
```

## ⚡ Características

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
- ✅ **Health Checks** - Monitoramento da aplicação
- ✅ **Docker Support** - Containerização pronta para produção
- ✅ **Configuração Segura** - Sistema automático de secrets
