using CoreTestBase.Helpers;

namespace CoreTestBase.Unit
{
    public class UserServiceTests
    {
        private readonly Mock<IRepositoryBase<User>> _mockRepository;
        private readonly UserService _userService;
        private readonly List<User> _sampleUsers;

        public UserServiceTests()
        {
            _sampleUsers = MockRepositoryFactory.CreateSampleUsers();
            _mockRepository = MockRepositoryFactory.CreateUserRepositoryWithData(_sampleUsers);
            _userService = new UserService(_mockRepository.Object);
        }

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_ShouldReturnUser_WhenUserIsValid()
        {
            // Arrange
            var newUser = new User
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Name = "New User",
                PasswordHash = "newhash"
            };

            // Act
            var result = await _userService.AddAsync(newUser);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Username.Should().Be("newuser");
            result.Email.Should().Be("newuser@example.com");
            result.Name.Should().Be("New User");
            
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldAssignNewId_WhenUserIdIsZero()
        {
            // Arrange
            var newUser = new User
            {
                Id = 0,
                Username = "testuser",
                Email = "test@example.com",
                Name = "Test User"
            };

            // Act
            var result = await _userService.AddAsync(newUser);

            // Assert
            result.Id.Should().BeGreaterThan(3); // Should be higher than existing users
            _sampleUsers.Should().Contain(u => u.Id == result.Id);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var existingUserId = 1;

            // Act
            var result = await _userService.GetByIdAsync(existingUserId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(existingUserId);
            result.Username.Should().Be("user1");
            result.Email.Should().Be("user1@example.com");
            
            _mockRepository.Verify(r => r.GetByIdAsync(existingUserId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = 999;

            // Act
            var result = await _userService.GetByIdAsync(nonExistentUserId);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(r => r.GetByIdAsync(nonExistentUserId), Times.Once);
        }

        [Theory]
        [InlineData(1, "user1")]
        [InlineData(2, "user2")]
        [InlineData(3, "user3")]
        public async Task GetByIdAsync_ShouldReturnCorrectUser_ForValidIds(int userId, string expectedUsername)
        {
            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(userId);
            result.Username.Should().Be(expectedUsername);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers_WhenUsersExist()
        {
            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain(u => u.Username == "user1");
            result.Should().Contain(u => u.Username == "user2");
            result.Should().Contain(u => u.Username == "user3");
            
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var emptyMock = MockRepositoryFactory.CreateUserRepositoryWithData(new List<User>());
            var emptyUserService = new UserService(emptyMock.Object);

            // Act
            var result = await emptyUserService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedUser_WhenUserExists()
        {
            // Arrange
            var userToUpdate = new User
            {
                Id = 1,
                Username = "updateduser",
                Email = "updated@example.com",
                Name = "Updated User",
                PasswordHash = "updatedhash"
            };

            // Act
            var result = await _userService.UpdateAsync(userToUpdate);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Username.Should().Be("updateduser");
            result.Email.Should().Be("updated@example.com");
            result.Name.Should().Be("Updated User");
            
            // Verify the user in the list was actually updated
            var updatedInList = _sampleUsers.First(u => u.Id == 1);
            updatedInList.Username.Should().Be("updateduser");
            updatedInList.Email.Should().Be("updated@example.com");
            
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallRepository_WithCorrectUser()
        {
            // Arrange
            var userToUpdate = new User { Id = 2, Username = "modified", Email = "modified@test.com" };

            // Act
            await _userService.UpdateAsync(userToUpdate);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(userToUpdate), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var userIdToDelete = 1;
            var initialCount = _sampleUsers.Count;

            // Act
            var result = await _userService.DeleteAsync(userIdToDelete);

            // Assert
            result.Should().BeTrue();
            _sampleUsers.Should().HaveCount(initialCount - 1);
            _sampleUsers.Should().NotContain(u => u.Id == userIdToDelete);
            
            _mockRepository.Verify(r => r.DeleteAsync(userIdToDelete), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = 999;
            var initialCount = _sampleUsers.Count;

            // Act
            var result = await _userService.DeleteAsync(nonExistentUserId);

            // Assert
            result.Should().BeFalse();
            _sampleUsers.Should().HaveCount(initialCount); // Count should remain the same
            
            _mockRepository.Verify(r => r.DeleteAsync(nonExistentUserId), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task DeleteAsync_ShouldRemoveCorrectUser_ForValidIds(int userId)
        {
            // Arrange
            var initialCount = _sampleUsers.Count;
            var userToDelete = _sampleUsers.First(u => u.Id == userId);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            result.Should().BeTrue();
            _sampleUsers.Should().HaveCount(initialCount - 1);
            _sampleUsers.Should().NotContain(userToDelete);
        }

        #endregion

        #region Edge Cases and Security

        [Fact]
        public async Task AddAsync_ShouldPreserveSecurityStamp_WhenUserHasOne()
        {
            // Arrange
            var userWithSecurityStamp = new User
            {
                Username = "secureuser",
                Email = "secure@example.com",
                Name = "Secure User",
                SecurityStamp = "custom-security-stamp"
            };

            // Act
            var result = await _userService.AddAsync(userWithSecurityStamp);

            // Assert
            result.SecurityStamp.Should().Be("custom-security-stamp");
        }

        [Fact]
        public async Task UpdateAsync_ShouldHandleSecurityStampRefresh()
        {
            // Arrange
            var user = _sampleUsers.First();
            var originalSecurityStamp = user.SecurityStamp;
            
            // Simulate refreshing security stamp
            user.RefreshSecurityStamp();

            // Act
            var result = await _userService.UpdateAsync(user);

            // Assert
            result.SecurityStamp.Should().NotBe(originalSecurityStamp);
            result.SecurityStamp.Should().NotBeNullOrEmpty();
        }

        #endregion
    }
}
