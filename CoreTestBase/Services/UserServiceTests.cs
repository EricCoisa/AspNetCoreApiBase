namespace CoreTestBase.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IRepositoryBase<User>> _mockRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockRepository = new Mock<IRepositoryBase<User>>();
            _userService = new UserService(_mockRepository.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnUser_WhenUserIsValid()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                Name = "Test User",
                PasswordHash = "hashedpassword"
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
                          .ReturnsAsync(user);

            // Act
            var result = await _userService.AddAsync(user);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Username.Should().Be("testuser");
            result.Email.Should().Be("test@example.com");
            result.Name.Should().Be("Test User");
            
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldCallRepositoryOnce_WhenCalled()
        {
            // Arrange
            var user = new User
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Name = "New User"
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
                          .ReturnsAsync(user);

            // Act
            await _userService.AddAsync(user);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(user), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new User
            {
                Id = userId,
                Username = "existinguser",
                Email = "existing@example.com",
                Name = "Existing User"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(userId))
                          .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(userId);
            result.Username.Should().Be("existinguser");
            
            _mockRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999;
            
            _mockRepository.Setup(r => r.GetByIdAsync(userId))
                          .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.Should().BeNull();
            
            _mockRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenUserIsDeleted()
        {
            // Arrange
            var userId = 1;
            
            _mockRepository.Setup(r => r.DeleteAsync(userId))
                          .ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            result.Should().BeTrue();
            
            _mockRepository.Verify(r => r.DeleteAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "user1@example.com", Name = "User 1" },
                new User { Id = 2, Username = "user2", Email = "user2@example.com", Name = "User 2" }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(u => u.Username == "user1");
            result.Should().Contain(u => u.Username == "user2");
            
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
