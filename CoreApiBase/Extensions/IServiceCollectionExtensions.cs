using Microsoft.Extensions.DependencyInjection;
using CoreDomainBase.Interfaces.Repositories;
using CoreDomainBase.Repositories;
using CoreDomainBase.Interfaces.Services;
using CoreDomainBase.Services;
using CoreApiBase.Services;
using CoreApiBase.Configurations;
using CoreApiBase.HealthChecks;

namespace CoreApiBase.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adiciona as dependências do projeto (repositórios, serviços, etc.).
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service collection para chaining</returns>
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped(typeof(IServicesBase<>), typeof(ServiceBase<>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<AuthService>();
            return services;
        }

        /// <summary>
        /// Configura e valida todas as configurações da aplicação usando Options Pattern.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection para chaining</returns>
        /// <exception cref="InvalidOperationException">Quando configurações obrigatórias estão inválidas</exception>
        public static IServiceCollection AddAndValidateConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurar e validar JwtSettings
            var jwtSection = configuration.GetSection(JwtSettings.SectionName);
            services.Configure<JwtSettings>(jwtSection);
            
            var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();
            jwtSettings.ValidateConfiguration(JwtSettings.SectionName);

            // Configurar e validar DatabaseSettings
            var dbSection = configuration.GetSection(DatabaseSettings.SectionName);
            var dbSettings = dbSection.Get<DatabaseSettings>() ?? new DatabaseSettings();
            
            // Para DatabaseSettings, usar ConnectionString do appsettings se não estiver na seção específica
            if (string.IsNullOrEmpty(dbSettings.ConnectionString))
            {
                dbSettings.ConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            }
            
            // Configurar o Options com a ConnectionString correta
            services.Configure<DatabaseSettings>(options =>
            {
                dbSection.Bind(options);
                if (string.IsNullOrEmpty(options.ConnectionString))
                {
                    options.ConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
                }
            });
            
            dbSettings.ValidateConfiguration(DatabaseSettings.SectionName);

            // Configurar e validar CorsSettings
            var corsSection = configuration.GetSection(CorsSettings.SectionName);
            services.Configure<CorsSettings>(corsSection);
            
            var corsSettings = corsSection.Get<CorsSettings>() ?? new CorsSettings();
            corsSettings.ValidateConfiguration(CorsSettings.SectionName);

            return services;
        }

        /// <summary>
        /// Adiciona health checks customizados da aplicação.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service collection para chaining</returns>
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<ConfigHealthCheck>("config", tags: new[] { "config", "startup" });

            return services;
        }
    }
}
