# CoreApiBase

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=flat&logo=docker)](https://docker.com)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/Tests-40%2B-brightgreen)](CoreTestBase/)

> **Starter template para APIs REST em ASP.NET Core 8.0 com autenticação JWT, Docker e estrutura completa de testes.**

## 🚀 Características

- ✅ **ASP.NET Core 8.0** com Entity Framework
- ✅ **Autenticação JWT** com refresh tokens
- ✅ **SQLite** pré-configurado (local) 
- ✅ **Docker** pronto para produção
- ✅ **Swagger/OpenAPI** documentação automática
- ✅ **AutoMapper** para DTOs
- ✅ **Health Checks** integrados
- ✅ **40+ Testes** (Unit, Integration, Contract)
- ✅ **Sistema de configuração segura** (User Secrets, Docker Secrets)

## 🏃‍♂️ Quick Start

### Desenvolvimento Local

```bash
# 1. Clone o repositório
git clone https://github.com/EricCoisa/AspNetCoreApiBase.git
cd AspNetCoreApiBase

# 2. Configure automaticamente (gera chaves JWT, User Secrets)
setup-configuration.bat development  # Windows
./setup-configuration.sh development # Linux/Mac

# 3. Execute
dotnet run --project CoreApiBase

# 4. Acesse
# API: http://localhost:5099
# Swagger: http://localhost:5099/swagger
```

### Com Docker

```bash
# 1. Configure para Docker
setup-configuration.bat docker  # Windows
./setup-configuration.sh docker # Linux/Mac

# 2. Execute
docker-compose up --build

# 3. Acesse
# API: http://localhost:8080
# Swagger: http://localhost:8080/swagger
```

## 📋 API Endpoints

### Authentication
- `POST /Authentication/register` - Registrar usuário
- `POST /Authentication/login` - Login
- `GET /Authentication/profile` - Perfil (🔒 JWT)

### Users (Admin)
- `GET /User` - Listar usuários (🔒 Admin)
- `POST /User` - Criar usuário (🔒 Admin)
- `PUT /User/{id}` - Atualizar usuário (🔒 Admin)
- `DELETE /User/{id}` - Deletar usuário (🔒 Admin)

### Health Checks
- `GET /health` - Status geral
- `GET /health/ready` - Prontidão
- `GET /health/live` - Vitalidade

## 🔧 Configuração

### Scripts Automáticos
```bash
# Desenvolvimento (User Secrets + SQLite local)
setup-configuration.bat development

# Docker (Docker Secrets)
setup-configuration.bat docker

# Produção (Environment Variables)
setup-configuration.bat production

# Limpar configurações
cleanup-secrets.bat
```

### Configurações Necessárias
- `JWT_SECRET_KEY` - Chave secreta JWT (gerada automaticamente)
- `DATABASE_CONNECTION_STRING` - String de conexão do banco
- `CORS_ALLOWED_ORIGINS` - Origens permitidas para CORS

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Apenas testes unitários
dotnet test --filter "Category=Unit"

# Apenas testes de integração
dotnet test --filter "Category=Integration"
```

### Cobertura
- **18 testes unitários** - UserService, repositories
- **12 testes de integração** - Health checks, API endpoints
- **10+ testes de contrato** - Snapshots de resposta da API

## 🐳 Deploy

### Docker Compose
```bash
docker-compose up -d --build
```

### IIS (Windows)
```bash
dotnet publish CoreApiBase -c Release -o ./publish
# Configure no IIS Manager
```

## 🏗️ Arquitetura

```
CoreApiBase/          # 🎯 API Layer
├── Controllers/      # Controladores REST
├── Application/      # DTOs e Profiles
├── Configurations/   # Configurações
├── Middlewares/      # Middlewares customizados
└── Utils/           # Utilitários

CoreDomainBase/       # 🏢 Domain & Data Layer
├── Entities/         # Entidades do domínio
├── Services/         # Lógica de negócio
├── Repositories/     # Acesso a dados
├── Interfaces/       # Contratos
└── Data/            # DbContext e configurações

CoreTestBase/         # 🧪 Tests
├── Unit/            # Testes unitários
├── Integration/     # Testes de integração
└── Contract/        # Testes de contrato
```

## 🔐 Segurança

- **JWT Authentication** com Security Stamp
- **Role-based Authorization** (User, Admin)
- **CORS** configurável por ambiente
- **Docker Secrets** para produção
- **User Secrets** para desenvolvimento

## 🛠️ Tecnologias

- .NET 8.0 / ASP.NET Core
- Entity Framework Core 9.0
- SQLite (desenvolvimento)
- AutoMapper 12.0
- Swagger/OpenAPI
- xUnit + Moq + FluentAssertions
- Docker & Docker Compose

## 📖 Exemplos

### Registro e Login
```bash
# Registrar
curl -X POST http://localhost:5099/Authentication/register \
  -H "Content-Type: application/json" \
  -d '{"name":"name","email":"email","password":"password"}'

# Login
curl -X POST http://localhost:5099/Authentication/login \
  -H "Content-Type: application/json" \
  -d '{"email":"email","password":"password"}'
```

### Health Check
```bash
curl http://localhost:5099/health
```

## 🤝 Contribuindo

1. Fork do projeto
2. Crie sua feature branch (`git checkout -b feature/AmazingFeature`)
3. Configure o ambiente (`setup-configuration.bat development`)
4. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
5. Push para a branch (`git push origin feature/AmazingFeature`)
6. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT.

---

**Desenvolvido com ❤️ por [EricCoisa](https://github.com/EricCoisa)**
