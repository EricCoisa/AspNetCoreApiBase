namespace CoreTestBase.Helpers
{
    public static class MockRepositoryFactory
    {
        public static Mock<IRepositoryBase<T>> CreateMockRepository<T>() where T : class
        {
            return new Mock<IRepositoryBase<T>>();
        }

        public static Mock<IRepositoryBase<User>> CreateUserRepositoryWithData(List<User> users)
        {
            var mock = new Mock<IRepositoryBase<User>>();
            
            // Setup GetAllAsync
            mock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
            
            // Setup GetByIdAsync
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => users.FirstOrDefault(u => u.Id == id));
            
            // Setup AddAsync
            mock.Setup(r => r.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) => 
                {
                    if (user.Id == 0)
                        user.Id = users.Max(u => u.Id) + 1;
                    users.Add(user);
                    return user;
                });
            
            // Setup UpdateAsync
            mock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) => 
                {
                    var existing = users.FirstOrDefault(u => u.Id == user.Id);
                    if (existing != null)
                    {
                        existing.Username = user.Username;
                        existing.Email = user.Email;
                        existing.Name = user.Name;
                        existing.PasswordHash = user.PasswordHash;
                        existing.Role = user.Role;
                    }
                    return user;
                });
            
            // Setup DeleteAsync
            mock.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => 
                {
                    var user = users.FirstOrDefault(u => u.Id == id);
                    if (user != null)
                    {
                        users.Remove(user);
                        return true;
                    }
                    return false;
                });
            
            return mock;
        }

        public static List<User> CreateSampleUsers()
        {
            return new List<User>
            {
                new User 
                { 
                    Id = 1, 
                    Username = "user1", 
                    Email = "user1@example.com", 
                    Name = "User One",
                    PasswordHash = "hash1"
                },
                new User 
                { 
                    Id = 2, 
                    Username = "user2", 
                    Email = "user2@example.com", 
                    Name = "User Two",
                    PasswordHash = "hash2"
                },
                new User 
                { 
                    Id = 3, 
                    Username = "user3", 
                    Email = "user3@example.com", 
                    Name = "User Three",
                    PasswordHash = "hash3"
                }
            };
        }
    }
}
