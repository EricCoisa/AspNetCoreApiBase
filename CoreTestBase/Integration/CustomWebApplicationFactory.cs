using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using CoreDomainBase.Data;
using Microsoft.AspNetCore.Builder;

namespace CoreTestBase.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseStartup<TestStartup>();
            
            // Configurar o content root manualmente para evitar o problema de solution root
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Remove configurações existentes
                config.Sources.Clear();
                
                // Adicionar configurações específicas para teste
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Database:UseInMemory", "true"},
                    {"Database:ConnectionString", ""},
                    {"Jwt:SecretKey", "test-secret-key-for-integration-tests-must-be-long-enough"},
                    {"Jwt:Issuer", "TestIssuer"},
                    {"Jwt:Audience", "TestAudience"},
                    {"Cors:Origins", "http://localhost:3000"},
                    {"Environment", "Testing"}
                });
            });

            builder.ConfigureServices(services =>
            {
                // Remove o contexto de banco de dados existente se houver
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona banco de dados em memória para testes
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Adicionar controladores do CoreApiBase
                services.AddControllers()
                        .AddApplicationPart(typeof(CoreApiBase.Controllers.HealthController).Assembly);

                // Health checks específicos para testes
                services.AddHealthChecks();

                // Build do service provider para inicializar o banco
                var serviceProvider = services.BuildServiceProvider();
                
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetService<AppDbContext>();
                
                if (context != null)
                {
                    // Garante que o banco é criado
                    context.Database.EnsureCreated();
                    
                    // Seed de dados para teste, se necessário
                    SeedTestData(context);
                }
            });

            builder.UseEnvironment("Testing");
        }

        private static void SeedTestData(AppDbContext context)
        {
            // Adicionar dados de teste se necessário
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Username = "testuser1",
                        Email = "test1@example.com",
                        Name = "Test User 1",
                        PasswordHash = "hashedpassword1",
                        Role = CoreDomainBase.Enums.Roles.User
                    },
                    new User
                    {
                        Username = "testuser2",
                        Email = "test2@example.com",
                        Name = "Test User 2",
                        PasswordHash = "hashedpassword2",
                        Role = CoreDomainBase.Enums.Roles.Admin
                    }
                );
                
                context.SaveChanges();
            }
        }
    }
}
