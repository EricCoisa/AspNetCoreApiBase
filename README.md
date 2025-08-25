# ASP.NET Core API Base - StarterPack

🚀 **Projeto base para criação rápida de APIs ASP.NET Core com Entity Framework, AutoMapper e SQLite**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0.8-green.svg)](https://docs.microsoft.com/ef/)
[![SQLite](https://img.shields.io/badge/SQLite-Database-lightgrey.svg)](https://www.sqlite.org/)
[![AutoMapper](https://img.shields.io/badge/AutoMapper-12.0.1-orange.svg)](https://automapper.org/)

## 📋 Sobre o Projeto

Este projeto é um **StarterPack** completo para desenvolvimento de APIs ASP.NET Core, fornecendo uma base sólida e organizada para acelerar o desenvolvimento de novos projetos. Inclui configurações essenciais, estrutura de pastas bem definida e implementações de CRUD prontas para uso.

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

## 🚀 Como Usar

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

### Instalação

1. **Clone o repositório**
   ```bash
   git clone https://github.com/EricCoisa/AspNetCoreApiBase.git
   cd AspNetCoreApiBase
   ```

2. **Restaure os pacotes**
   ```bash
   dotnet restore
   ```

3. **Execute as migrations**
   ```bash
   dotnet ef database update --project CoreApiBase
   ```

4. **Execute o projeto**
   ```bash
   dotnet run --project CoreApiBase
   ```

5. **Acesse a API**
   - API: `http://localhost:5099`
   - Swagger: `http://localhost:5099/swagger`

## 🎯 Endpoints Disponíveis

### Users API
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/User` | Lista todos os usuários |
| GET | `/User/{id}` | Busca usuário por ID |
| POST | `/User` | Cria novo usuário |
| PUT | `/User/{id}` | Atualiza usuário |
| DELETE | `/User/{id}` | Remove usuário |

### Exemplo de Payload
```json
{
  "id": 1,
  "name": "João Silva"
}
```

## 🔧 Configuração

### Connection String
Edite o arquivo `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=appdb.sqlite"
  }
}
```

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

## 🧪 Testes

Execute os testes do projeto:
```bash
dotnet test
```

## 📦 Pacotes Incluídos

| Pacote | Versão | Propósito |
|--------|--------|-----------|
| Microsoft.EntityFrameworkCore.Sqlite | 9.0.8 | Provider SQLite para EF Core |
| Microsoft.EntityFrameworkCore.Design | 9.0.8 | Ferramentas de design do EF |
| AutoMapper | 12.0.1 | Mapeamento objeto-objeto |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | Integração AutoMapper com DI |
| Swashbuckle.AspNetCore | 6.6.2 | Documentação Swagger |

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

---

⭐ **Se este projeto foi útil para você, considere dar uma estrela!**
