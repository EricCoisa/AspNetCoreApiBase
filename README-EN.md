# CoreApiBase

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=flat&logo=docker)](https://docker.com)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/Tests-40%2B-brightgreen)](CoreTestBase/)

> **Starter template for REST APIs in ASP.NET Core 8.0 with JWT authentication, Docker, and a complete test suite.**

## 🚀 Features

- ✅ **ASP.NET Core 8.0** with Entity Framework
- ✅ **JWT Authentication** with refresh tokens
- ✅ **SQLite** pre-configured (local)
- ✅ **Docker** ready for production
- ✅ **Swagger/OpenAPI** automatic documentation
- ✅ **AutoMapper** for DTOs
- ✅ **Integrated Health Checks**
- ✅ **40+ Tests** (Unit, Integration, Contract)
- ✅ **Secure configuration system** (User Secrets, Docker Secrets)

## 🏃‍♂️ Quick Start

### Local Development

```bash
# 1. Clone the repository
git clone https://github.com/EricCoisa/AspNetCoreApiBase.git
cd AspNetCoreApiBase

# 2. Auto-configure (generates JWT keys, User Secrets)
setup-configuration.bat development  # Windows
./setup-configuration.sh development # Linux/Mac

# 3. Run
dotnet run --project CoreApiBase

# 4. Access
# API: http://localhost:5099
# Swagger: http://localhost:5099/swagger
```

### With Docker

```bash
# 1. Configure for Docker
setup-configuration.bat docker  # Windows
./setup-configuration.sh docker # Linux/Mac

# 2. Run
docker-compose up --build

# 3. Access
# API: http://localhost:8080
# Swagger: http://localhost:8080/swagger
```

## 📋 API Endpoints

### Authentication
- `POST /Authentication/register` - Register user
- `POST /Authentication/login` - Login
- `GET /Authentication/profile` - Profile (🔒 JWT)

### Users (Admin)
- `GET /User` - List users (🔒 Admin)
- `POST /User` - Create user (🔒 Admin)
- `PUT /User/{id}` - Update user (🔒 Admin)
- `DELETE /User/{id}` - Delete user (🔒 Admin)

### Health Checks
- `GET /health` - General status
- `GET /health/ready` - Readiness
- `GET /health/live` - Liveness

## 🔧 Configuration

### Automated Scripts
```bash
# Development (User Secrets + local SQLite)
setup-configuration.bat development

# Docker (Docker Secrets)
setup-configuration.bat docker

# Production (Environment Variables)
setup-configuration.bat production

# Clean configuration
cleanup-secrets.bat
```

### Required Settings
- `JWT_SECRET_KEY` - JWT secret key (auto-generated)
- `DATABASE_CONNECTION_STRING` - Database connection string
- `CORS_ALLOWED_ORIGINS` - Allowed origins for CORS

## 🧪 Tests

```bash
# Run all tests
dotnet test

# Only unit tests
dotnet test --filter "Category=Unit"

# Only integration tests
dotnet test --filter "Category=Integration"
```

### Coverage
- **18 unit tests** - UserService, repositories
- **12 integration tests** - Health checks, API endpoints
- **10+ contract tests** - API response snapshots

## 🐳 Deploy

### Docker Compose
```bash
docker-compose up -d --build
```

### IIS (Windows)
```bash
dotnet publish CoreApiBase -c Release -o ./publish
# Configure in IIS Manager
```

## 🏗️ Architecture

```
CoreApiBase/          # 🎯 API Layer
├── Controllers/      # REST Controllers
├── Application/      # DTOs and Profiles
├── Configurations/   # Configurations
├── Middlewares/      # Custom middlewares
└── Utils/           # Utilities

CoreDomainBase/       # 🏢 Domain & Data Layer
├── Entities/         # Domain entities
├── Services/         # Business logic
├── Repositories/     # Data access
├── Interfaces/       # Contracts
└── Data/            # DbContext and configs

CoreTestBase/         # 🧪 Tests
├── Unit/            # Unit tests
├── Integration/     # Integration tests
└── Contract/        # Contract tests
```

## 🔐 Security

- **JWT Authentication** with Security Stamp
- **Role-based Authorization** (User, Admin)
- **CORS** configurable per environment
- **Docker Secrets** for production
- **User Secrets** for development

## 🛠️ Technologies

- .NET 8.0 / ASP.NET Core
- Entity Framework Core 9.0
- SQLite (development)
- AutoMapper 12.0
- Swagger/OpenAPI
- xUnit + Moq + FluentAssertions
- Docker & Docker Compose

## 📖 Examples

### Register and Login
```bash
# Register
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

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Setup the environment (`setup-configuration.bat development`)
4. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
5. Push to the branch (`git push origin feature/AmazingFeature`)
6. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.

---

**Developed with ❤️ by [EricCoisa](https://github.com/EricCoisa)**
