using CoreDomainBase.Entities;
using CoreDomainBase.Interfaces.Services;
using CoreDomainBase.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreDomainBase.Services
{
    public class UserService : ServiceBase<User>
    {
        public UserService(IRepositoriesBase<User> repository) : base(repository)
        {
        }
    }
}
