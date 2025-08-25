using CoreDomainBase.Interfaces.Services;
using CoreDomainBase.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreDomainBase.Services
{
    public class ServiceBase<T> : IServicesBase<T> where T : class
    {
        private readonly IRepositoriesBase<T> _repository;

        public ServiceBase(IRepositoriesBase<T> repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<T>> GetAllAsync() => _repository.GetAllAsync();
        public Task<T?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task<T> AddAsync(T entity) => _repository.AddAsync(entity);
        public Task<T> UpdateAsync(T entity) => _repository.UpdateAsync(entity);
        public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
    }
}
