using Microsoft.Extensions.DependencyInjection;
using CoreDomainBase.Interfaces.Repositories;
using CoreDomainBase.Repositories;
using CoreDomainBase.Interfaces.Services;
using CoreDomainBase.Services;

namespace CoreApiBase.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepositoriesBase<>), typeof(RepositoriesBase<>));
            services.AddScoped(typeof(IServicesBase<>), typeof(ServiceBase<>));
            return services;
        }
    }
}
