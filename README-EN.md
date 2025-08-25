# ASP.NET Core API Base - StarterPack

🚀 **Base project for fast creation of ASP.NET Core APIs with Entity Framework, AutoMapper and SQLite**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0.8-green.svg)](https://docs.microsoft.com/ef/)
[![SQLite](https://img.shields.io/badge/SQLite-Database-lightgrey.svg)](https://www.sqlite.org/)
[![AutoMapper](https://img.shields.io/badge/AutoMapper-12.0.1-orange.svg)](https://automapper.org/)

## 📋 About the Project

This project is a complete **StarterPack** for ASP.NET Core API development, providing a solid and organized foundation to accelerate the development of new projects. It includes essential configurations, well-defined folder structure, and ready-to-use CRUD implementations.

## 🏗️ Architecture

The project follows a well-defined layered architecture:

```
CoreApiBase/          # 🎯 Presentation Layer (API)
├── Controllers/      # API Controllers
├── Application/      # DTOs and application models
├── Extensions/       # Extension methods for DI
├── Configurations/   # Project configurations
└── Middlewares/      # Custom middlewares

CoreDomainBase/       # 🏢 Domain and Data Layer
├── Entities/         # Domain entities
├── Services/         # Business services
├── Repositories/     # Data repositories
├── Interfaces/       # Contracts and interfaces
└── Data/            # Database context and EF configurations
    └── Configurations/ # Entity configurations

Tests/               # 🧪 Tests
├── UnitTests/       # Unit tests
└── IntegrationTests/ # Integration tests
```

## ⚡ Features

- ✅ **ASP.NET Core 8.0** - Modern and performant framework
- ✅ **Entity Framework Core** - ORM with pre-configured SQLite
- ✅ **AutoMapper** - Automatic mapping between entities and DTOs
- ✅ **Swagger/OpenAPI** - Automatic API documentation
- ✅ **Dependency Injection** - Configured and ready to use
- ✅ **Repository Pattern** - Generic repository implementation
- ✅ **Complete CRUD** - Functional example with User entity
- ✅ **Migrations** - Database version control
- ✅ **Clean Structure** - Clear separation of concerns

## 🚀 How to Use

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/EricCoisa/AspNetCoreApiBase.git
   cd AspNetCoreApiBase
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Run migrations**
   ```bash
   dotnet ef database update --project CoreApiBase
   ```

4. **Run the project**
   ```bash
   dotnet run --project CoreApiBase
   ```

5. **Access the API**
   - API: `http://localhost:5099`
   - Swagger: `http://localhost:5099/swagger`

## 🎯 Available Endpoints

### Users API
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/User` | List all users |
| GET | `/User/{id}` | Get user by ID |
| POST | `/User` | Create new user |
| PUT | `/User/{id}` | Update user |
| DELETE | `/User/{id}` | Delete user |

### Example Payload
```json
{
  "id": 1,
  "name": "John Doe"
}
```

## 🔧 Configuration

### Connection String
Edit the `appsettings.json` file:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=appdb.sqlite"
  }
}
```

### Adding New Entities

1. **Create the entity** in `CoreDomainBase/Entities/`
2. **Create the DTO** in `CoreApiBase/Application/DTOs/`
3. **Configure the mapping** in `CoreDomainBase/Data/Configurations/`
4. **Add to DbContext** in `CoreDomainBase/Data/AppDbContext.cs`
5. **Run the migration**:
   ```bash
   dotnet ef migrations add MigrationName --project CoreApiBase
   dotnet ef database update --project CoreApiBase
   ```

## 🧪 Testing

Run the project tests:
```bash
dotnet test
```

## 📦 Included Packages

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore.Sqlite | 9.0.8 | SQLite provider for EF Core |
| Microsoft.EntityFrameworkCore.Design | 9.0.8 | EF design-time tools |
| AutoMapper | 12.0.1 | Object-to-object mapping |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | AutoMapper integration with DI |
| Swashbuckle.AspNetCore | 6.6.2 | Swagger documentation |

## 🤝 Contributing

1. Fork the project
2. Create a feature branch (`git checkout -b feature/MyFeature`)
3. Commit your changes (`git commit -m 'Add new feature'`)
4. Push to the branch (`git push origin feature/MyFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## 🔗 Useful Links

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [AutoMapper](https://docs.automapper.org/)
- [Swagger/OpenAPI](https://swagger.io/)

---

⭐ **If this project was helpful to you, consider giving it a star!**