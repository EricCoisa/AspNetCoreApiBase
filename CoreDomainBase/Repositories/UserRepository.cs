using CoreDomainBase.Entities;
using CoreDomainBase.Interfaces.Repositories;
using CoreDomainBase.Data;

namespace CoreDomainBase.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}
