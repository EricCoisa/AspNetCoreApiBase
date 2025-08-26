namespace CoreTestBase.Services
{
    /// <summary>
    /// Exemplo simples de teste básico para UserService
    /// Este exemplo mostra como inicializar mocks e testar o método AddAsync
    /// </summary>
    public class UserServiceBasicTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddUserSuccessfully()
        {
            // Arrange - Configuração inicial
            var mockRepository = new Mock<IRepositoryBase<User>>();
            var userService = new UserService(mockRepository.Object);
            
            var newUser = new User
            {
                Id = 1,
                Username = "novoUsuario",
                Email = "novo@email.com",
                Name = "Usuário Novo"
            };

            // Configurando o mock para retornar o usuário quando AddAsync for chamado
            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                         .ReturnsAsync(newUser);

            // Act - Execução do método sendo testado
            var resultado = await userService.AddAsync(newUser);

            // Assert - Verificações usando FluentAssertions
            resultado.Should().NotBeNull();
            resultado.Username.Should().Be("novoUsuario");
            resultado.Email.Should().Be("novo@email.com");
            
            // Verificando se o repository foi chamado uma vez
            mockRepository.Verify(repo => repo.AddAsync(newUser), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WithMinimalData_ShouldWork()
        {
            // Arrange
            var mockRepository = new Mock<IRepositoryBase<User>>();
            var userService = new UserService(mockRepository.Object);
            
            var user = new User { Username = "teste", Email = "teste@teste.com" };

            mockRepository.Setup(r => r.AddAsync(user)).ReturnsAsync(user);

            // Act
            var result = await userService.AddAsync(user);

            // Assert
            result.Should().Be(user);
            mockRepository.Verify(r => r.AddAsync(user), Times.Once);
        }
    }
}
