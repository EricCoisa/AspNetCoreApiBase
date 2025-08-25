using Microsoft.Extensions.DependencyInjection;
using CoreDomainBase.Interfaces.Repositories;
using CoreDomainBase.Repositories;
using CoreDomainBase.Interfaces.Services;
using CoreDomainBase.Services;
using CoreApiBase.Services;

namespace CoreApiBase.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped(typeof(IServicesBase<>), typeof(ServiceBase<>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<AuthService>();
            return services;
        }
    }
}
