# CoreTestBase

Projeto de testes unitários para o CoreDomainBase.

## Tecnologias Utilizadas

- **xUnit**: Framework de testes
- **Moq**: Framework para criação de mocks
- **FluentAssertions**: Biblioteca para asserções fluentes

## Estrutura

```
CoreTestBase/
├── Services/
│   └── UserServiceTests.cs    # Testes para UserService
├── GlobalUsings.cs            # Usings globais
└── CoreTestBase.csproj        # Configuração do projeto
```

## Como Executar

```bash
dotnet test
```

## Padrão dos Testes

Os testes seguem o padrão AAA (Arrange, Act, Assert):

1. **Arrange**: Configuração dos mocks e dados de teste
2. **Act**: Execução do método sendo testado
3. **Assert**: Verificação dos resultados usando FluentAssertions

## Exemplo de Mock

```csharp
// Configuração do mock
_mockRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
              .ReturnsAsync(user);

// Verificação de chamada
_mockRepository.Verify(r => r.AddAsync(user), Times.Once);
```
