using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace CoreApiBase.Configurations
{
    public static class AutoMapperConfig
    {
        public static IServiceCollection AddProjectAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}
