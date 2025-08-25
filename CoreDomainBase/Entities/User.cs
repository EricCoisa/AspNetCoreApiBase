using CoreDomainBase.Enums;

namespace CoreDomainBase.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Roles Role { get; set; } = Roles.User;
        public string Name { get; set; } = string.Empty;
    }
}
