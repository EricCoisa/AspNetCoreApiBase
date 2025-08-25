using CoreDomainBase.Entities;
using CoreDomainBase.Interfaces.Repositories;
using CoreDomainBase.Data;

namespace CoreDomainBase.Repositories
{
    public class UserRepositories : RepositoriesBase<User>
    {
        public UserRepositories(AppDbContext context) : base(context)
        {
        }
    }
}
