# Exemplo de Uso - Testes Unitários com xUnit, Moq e FluentAssertions

## Como Inicializar o Mock do IRepositoryBase<User>

```csharp
// 1. Criar o mock da interface
var mockRepository = new Mock<IRepositoryBase<User>>();

// 2. Passar o mock para o UserService
var userService = new UserService(mockRepository.Object);
```

## Como Configurar o Comportamento do Mock

```csharp
// Configurar para retornar um usuário específico
var user = new User { Id = 1, Username = "teste" };
mockRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
              .ReturnsAsync(user);

// Configurar para aceitar um usuário específico
mockRepository.Setup(repo => repo.AddAsync(user))
              .ReturnsAsync(user);
```

## Como Usar FluentAssertions

```csharp
// Verificações básicas
result.Should().NotBeNull();
result.Id.Should().Be(1);
result.Username.Should().Be("teste");

// Verificações de coleções
users.Should().HaveCount(2);
users.Should().Contain(u => u.Username == "user1");
```

## Como Verificar Chamadas do Mock

```csharp
// Verificar se um método foi chamado uma vez
mockRepository.Verify(repo => repo.AddAsync(user), Times.Once);

// Verificar se foi chamado com qualquer parâmetro
mockRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);

// Verificar se nunca foi chamado
mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
```

## Estrutura Básica de um Teste

```csharp
[Fact]
public async Task NomeDoMetodo_Cenario_ResultadoEsperado()
{
    // Arrange - Configuração
    var mockRepository = new Mock<IRepositoryBase<User>>();
    var userService = new UserService(mockRepository.Object);
    var user = new User { /* dados */ };
    
    mockRepository.Setup(/* configuração do comportamento */);

    // Act - Execução
    var result = await userService.AddAsync(user);

    // Assert - Verificações
    result.Should().NotBeNull();
    mockRepository.Verify(/* verificação de chamadas */);
}
```
