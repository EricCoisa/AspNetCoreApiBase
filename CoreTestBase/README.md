# CoreTestBase

Projeto de testes unitários e de integração para o CoreDomainBase e CoreApiBase.

## 🏗️ Estrutura Organizada

```
CoreTestBase/
├── Unit/                           # Testes unitários isolados
│   └── UserServiceTests.cs         # Testes do UserService com mocks
├── Integration/                    # Testes de integração com WebAPI
│   ├── CustomWebApplicationFactory.cs  # Factory customizada para testes
│   ├── TestStartup.cs              # Startup simplificado para testes
│   └── HealthTests.cs              # Testes do endpoint /health
├── Helpers/                        # Utilitários e factories reutilizáveis
│   └── MockRepositoryFactory.cs    # Factory para criação de mocks
├── GlobalUsings.cs                 # Usings globais
├── CoreTestBase.csproj             # Configuração do projeto
├── README.md                       # Esta documentação
└── EXEMPLOS.md                     # Guia prático de exemplos
```

## 🛠️ Tecnologias Utilizadas

### Frameworks de Teste
- **xUnit**: Framework principal de testes
- **Moq**: Framework para criação de mocks e stubs
- **FluentAssertions**: Biblioteca para asserções fluentes e legíveis

### Testes de Integração
- **Microsoft.AspNetCore.Mvc.Testing**: WebApplicationFactory para testes de API
- **Microsoft.EntityFrameworkCore.InMemory**: Banco de dados em memória

## 🧪 Tipos de Testes

### 1. Testes Unitários (`Unit/`)
- Testam componentes isoladamente
- Usam mocks para dependências
- Focam na lógica de negócio
- Execução rápida

**Exemplo**:
```csharp
[Fact]
public async Task AddAsync_ShouldReturnUser_WhenUserIsValid()
{
    // Arrange
    var mockRepository = MockRepositoryFactory.CreateUserRepositoryWithData(_sampleUsers);
    var userService = new UserService(mockRepository.Object);
    var newUser = new User { Username = "test", Email = "test@test.com" };

    // Act
    var result = await userService.AddAsync(newUser);

    // Assert
    result.Should().NotBeNull();
    result.Username.Should().Be("test");
    mockRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
}
```

### 2. Testes de Integração (`Integration/`)
- Testam endpoints da API
- Usam banco de dados em memória
- Testam o comportamento end-to-end
- Verificam serialização/deserialização

**Exemplo**:
```csharp
[Fact]
public async Task GetHealth_ShouldReturnOk_WhenApplicationIsHealthy()
{
    // Act
    var response = await _client.GetAsync("/api/health");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

## 🏭 MockRepositoryFactory - Reutilização de Mocks

A classe `MockRepositoryFactory` centraliza a criação de mocks reutilizáveis:

### Funcionalidades
- **Mock simples**: `CreateMockRepository<T>()`
- **Mock com dados**: `CreateUserRepositoryWithData(users)`
- **Dados de exemplo**: `CreateSampleUsers()`
- **Comportamento realista**: Simula operações CRUD reais

### Vantagens
- ✅ **Consistência**: Mocks padronizados em todos os testes
- ✅ **Reutilização**: Evita código duplicado
- ✅ **Manutenção**: Mudanças centralizadas
- ✅ **Realismo**: Comportamento similar ao repositório real

## 🚀 Como Executar

### Todos os Testes
```bash
dotnet test
```

### Apenas Testes Unitários
```bash
dotnet test --filter "UserServiceTests"
```

### Apenas Testes de Integração
```bash
dotnet test --filter "HealthTests"
```

### Com Verbosidade Detalhada
```bash
dotnet test --logger "console;verbosity=detailed"
```

## 📊 Status dos Testes

### ✅ Testes Unitários (18/18)
- **UserService**: 18 testes cobrindo todos os métodos CRUD
- **Mocks**: Factory reutilizável funcionando
- **FluentAssertions**: Asserções claras e legíveis
- **Coverage**: Todos os métodos do service layer

### ⚠️ Testes de Integração (0/12)
- **Status**: Em desenvolvimento
- **Problema**: Configuração do WebApplicationFactory
- **Próximos passos**: Simplificar setup para endpoints básicos

## 🎯 Padrões Implementados

### Testes Unitários
1. **AAA Pattern**: Arrange, Act, Assert
2. **Mock Isolation**: Dependências sempre mockadas
3. **Factory Pattern**: MockRepositoryFactory para reutilização
4. **Fluent Assertions**: Sintaxe legível e clara
5. **Theory/InlineData**: Testes parametrizados

### Organização
1. **Separation of Concerns**: Unit vs Integration
2. **Helper Classes**: Utilitários reutilizáveis
3. **Global Usings**: Imports centralizados
4. **Consistent Naming**: Padrão `MethodName_Scenario_ExpectedResult`

## 📈 Cobertura Atual

### UserService - 100% dos métodos
- ✅ `AddAsync()` - 3 cenários
- ✅ `GetByIdAsync()` - 4 cenários  
- ✅ `GetAllAsync()` - 2 cenários
- ✅ `UpdateAsync()` - 2 cenários
- ✅ `DeleteAsync()` - 4 cenários
- ✅ **Edge Cases** - 3 cenários especiais

### Cenários Testados
- ✅ Operações bem-sucedidas
- ✅ Entidades não encontradas
- ✅ Validações de entrada
- ✅ Comportamentos edge cases
- ✅ Verificação de chamadas ao repositório

## 🔧 Configuração do Projeto

### Dependências Principais
```xml
<PackageReference Include="xunit" Version="2.6.1" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.8" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.8" />
```

### Referências de Projeto
- `CoreDomainBase` - Entidades e serviços
- `CoreApiBase` - Controllers e endpoints

## 📝 Próximos Passos

### 🎯 Prioridade Alta
1. **Corrigir testes de integração**: Resolver problema de WebApplicationFactory
2. **Expandir cobertura**: Adicionar testes para outros services
3. **Testes de performance**: Benchmarks básicos

### 🎯 Prioridade Média
4. **Testes de validação**: Cenários de erro e exceções
5. **Testes de autorização**: JWT e roles
6. **Testes de banco**: Migrations e seeds

### 🎯 Prioridade Baixa
7. **Code coverage**: Relatórios de cobertura
8. **CI/CD Integration**: Pipeline automatizado
9. **Mutation testing**: Testes de qualidade dos testes

## 📚 Recursos Adicionais

- **EXEMPLOS.md**: Guia prático com exemplos de código
- **Documentação xUnit**: https://xunit.net/
- **Documentação Moq**: https://github.com/moq/moq
- **Documentação FluentAssertions**: https://fluentassertions.com/
