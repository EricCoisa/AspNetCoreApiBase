using CoreDomainBase.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using CoreApiBase.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Localization;

namespace CoreTestBase.Integration
{
    /// <summary>
    /// Classe base para testes de integração, configurando WebApplicationFactory com banco em memória
    /// </summary>
    public class IntegrationTestBase : IClassFixture<IntegrationWebApplicationFactory>
    {
        protected readonly IntegrationWebApplicationFactory Factory;
        protected readonly HttpClient Client;

        public IntegrationTestBase(IntegrationWebApplicationFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
        }
    }

    /// <summary>
    /// WebApplicationFactory customizada para testes de integração
    /// </summary>
    public class IntegrationWebApplicationFactory : WebApplicationFactory<Program>
    {
        private static int _databaseId = 0;
        private static readonly string _dynamicJwtKey = GenerateValidJwtKey();

        /// <summary>
        /// Gera uma chave JWT válida e segura para testes
        /// </summary>
        private static string GenerateValidJwtKey()
        {
            // Gera uma chave de 64 caracteres (512 bits) usando caracteres seguros
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random(DateTime.Now.Millisecond);
            return new string(Enumerable.Repeat(chars, 64)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // IMPORTANTE: ConfigureAppConfiguration deve vir primeiro
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Configurações específicas para testes que passem na validação do health check
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Connection strings
                {"ConnectionStrings:DefaultConnection", "DataSource=:memory:"},
                
                // JWT Settings - configuração completa e válida com URLs e chave dinâmica
                {"JwtSettings:SecretKey", _dynamicJwtKey},
                {"JwtSettings:Issuer", "https://localhost:5001"},
                {"JwtSettings:Audience", "https://localhost:5001"},
                {"JwtSettings:ExpiryMinutes", "60"},
                {"JwtSettings:ExpirationTimeInMinutes", "60"},
                
                // Database Settings - configuração completa
                {"DatabaseSettings:ConnectionString", "DataSource=:memory:"},
                {"DatabaseSettings:Provider", "InMemory"},
                {"DatabaseSettings:AutoMigrate", "false"},
                {"DatabaseSettings:SeedData", "false"},
                {"DatabaseSettings:CommandTimeout", "30"},
                {"DatabaseSettings:LogLevel", "Warning"},
                {"DatabaseSettings:EnableSensitiveDataLogging", "true"},
                
                // CORS Settings - configuração detalhada para validação
                {"CorsSettings:AllowedOrigins:0", "http://localhost:3000"},
                {"CorsSettings:AllowedOrigins:1", "https://localhost:3001"},
                {"CorsSettings:AllowedMethods:0", "GET"},
                {"CorsSettings:AllowedMethods:1", "POST"},
                {"CorsSettings:AllowedMethods:2", "PUT"},
                {"CorsSettings:AllowedMethods:3", "DELETE"},
                {"CorsSettings:AllowedMethods:4", "OPTIONS"},
                {"CorsSettings:AllowedHeaders:0", "*"},
                {"CorsSettings:AllowCredentials", "false"},
                {"CorsSettings:PreflightMaxAge", "3600"},
                
                // Environment e cultura
                {"Environment", "Testing"},
                
                // Configurações de localização para inglês
                {"RequestLocalizationOptions:DefaultRequestCulture:Culture", "en-US"},
                {"RequestLocalizationOptions:DefaultRequestCulture:UICulture", "en-US"},
                {"RequestLocalizationOptions:SupportedCultures:0", "en-US"},
                {"RequestLocalizationOptions:SupportedUICultures:0", "en-US"}
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove todos os registros relacionados ao banco para evitar conflitos
            var servicesToRemoveDb = services.Where(s => 
                s.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                s.ServiceType == typeof(AppDbContext) ||
                s.ServiceType.IsGenericType && s.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>) ||
                s.ServiceType.FullName?.Contains("EntityFramework") == true ||
                s.ServiceType.FullName?.Contains("Microsoft.Data.Sqlite") == true ||
                s.ServiceType.FullName?.Contains("Sqlite") == true
            ).ToList();
            
            foreach (var serviceDb in servicesToRemoveDb)
            {
                services.Remove(serviceDb);
            }

            // Usar a chave JWT gerada dinamicamente
            var dynamicJwtKey = _dynamicJwtKey;
            
            // Remove o JwtSettings existente e adiciona um customizado para testes
            var jwtDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(JwtSettings));
            if (jwtDescriptor != null)
            {
                services.Remove(jwtDescriptor);
            }
            
            // Adiciona JwtSettings para testes com chave gerada dinamicamente
            var testJwtSettings = new JwtSettings
            {
                SecretKey = dynamicJwtKey,
                Issuer = "https://localhost:5001",
                Audience = "https://localhost:5001",
                ExpiryMinutes = 60
            };
            services.AddSingleton(testJwtSettings);

            // Remove COMPLETAMENTE todo o sistema de autenticação para testes
            var servicesToRemove = services.Where(s => 
                s.ServiceType.FullName?.Contains("Authentication") == true ||
                s.ServiceType.FullName?.Contains("Authorization") == true ||
                s.ServiceType.FullName?.Contains("JwtBearer") == true ||
                s.ServiceType.Name.Contains("Auth")).ToList();
                
            foreach (var service in servicesToRemove)
            {
                services.Remove(service);
            }
            
            // Adiciona sistema de autenticação sem validação para testes
            services.AddAuthentication("NoAuth").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("NoAuth", o => { });
            
            // Adiciona serviços de autorização com políticas que sempre permitem para testes
            services.AddAuthorization(options =>
            {
                // Define uma política padrão que sempre permite acesso
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(_ => true)
                    .Build();
                    
                // Todas as políticas sempre permitem para facilitar os testes
                options.AddPolicy(CoreDomainBase.Enums.Policy.AdminOnly.ToString(), policy => policy.RequireAssertion(_ => true));
                options.AddPolicy(CoreDomainBase.Enums.Policy.UserOrAdmin.ToString(), policy => policy.RequireAssertion(_ => true));
                options.AddPolicy("AdminOnly", policy => policy.RequireAssertion(_ => true));
                options.AddPolicy("UserOnly", policy => policy.RequireAssertion(_ => true));
                options.AddPolicy("RequireAdmin", policy => policy.RequireAssertion(_ => true));
                options.AddPolicy("RequireUser", policy => policy.RequireAssertion(_ => true));
            });

            // Mock do AuthService para os testes
            services.AddTransient<CoreApiBase.Services.AuthService>(provider =>
            {
                var jwtSettings = provider.GetRequiredService<JwtSettings>();
                return new CoreApiBase.Services.AuthService(jwtSettings);
            });

            // Configurar Options para health checks
            services.Configure<DatabaseSettings>(options =>
            {
                options.ConnectionString = "Data Source=:memory:";
                options.AutoMigrate = false;
                options.SeedData = false;
                options.EnableSensitiveDataLogging = true;
            });

            services.Configure<CorsSettings>(options =>
            {
                options.AllowedOrigins = new[] { "http://localhost:3000" };
                options.AllowCredentials = false;
            });

            // Adicionar health checks para testes
            services.AddHealthChecks();

            // Configurar localização para inglês nos testes
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en-US" };
                options.SetDefaultCulture("en-US")
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });

            // Adicionar controllers do CoreApiBase explicitamente
            services.AddControllers()
                .AddApplicationPart(typeof(CoreApiBase.Controllers.HealthController).Assembly);

            // Adiciona DbContext com banco em memória (único para cada teste)
            var databaseName = $"TestDatabase_{Interlocked.Increment(ref _databaseId)}_{Guid.NewGuid()}";
            
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName);
                options.EnableSensitiveDataLogging();
            });

            // Remove logs desnecessários para testes
            services.RemoveAll<ILoggerProvider>();
            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        });

        builder.UseEnvironment("Testing");
        
        // Configurar o pipeline de middleware específico para testes
        // Isso substitui a configuração padrão do Program.cs
        builder.Configure(app =>
        {
            // Pipeline simplificado para testes, sem middlewares que podem causar problemas
            app.UseRouting();
            
            // Configurar localização para inglês
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US"),
                SupportedCultures = new[] { new System.Globalization.CultureInfo("en-US") },
                SupportedUICultures = new[] { new System.Globalization.CultureInfo("en-US") }
            });
            
            // CORS para testes
            app.UseCors();
            
            // Autenticação customizada (TestAuthHandler)
            app.UseAuthentication();
            
            // Autorização (políticas que sempre permitem)
            app.UseAuthorization();
            
            // Mapear endpoints
            app.UseEndpoints(endpoints =>
            {
                // Mapear todos os controllers
                endpoints.MapControllers();
                
                // Mapear health checks tanto como middleware quanto controllers
                endpoints.MapHealthChecks("/health");
                endpoints.MapHealthChecks("/health/config");
            });
        });
    }        /// <summary>
        /// Inicializa o banco de dados com dados de teste
        /// </summary>
        public async Task<AppDbContext> InitializeDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            await context.Database.EnsureCreatedAsync();
            await SeedTestDataAsync(context);
            
            return context;
        }

        /// <summary>
        /// Popula o banco com dados de teste
        /// </summary>
        private static async Task SeedTestDataAsync(AppDbContext context)
        {
            if (!await context.Users.AnyAsync())
            {
                var testUsers = new[]
                {
                    new User
                    {
                        Id = 1,
                        Username = "testuser1",
                        Email = "test1@example.com",
                        Name = "Test User 1",
                        PasswordHash = "hashedpassword1",
                        Role = CoreDomainBase.Enums.Roles.User
                    },
                    new User
                    {
                        Id = 2,
                        Username = "testuser2",
                        Email = "test2@example.com",
                        Name = "Test User 2",
                        PasswordHash = "hashedpassword2",
                        Role = CoreDomainBase.Enums.Roles.Admin
                    },
                    new User
                    {
                        Id = 3,
                        Username = "testuser3",
                        Email = "test3@example.com",
                        Name = "Test User 3",
                        PasswordHash = "hashedpassword3",
                        Role = CoreDomainBase.Enums.Roles.User
                    }
                };

                context.Users.AddRange(testUsers);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Limpa o banco de dados para isolamento de testes
        /// </summary>
        public async Task ClearDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtém uma instância do contexto de banco para operações diretas
        /// </summary>
        public AppDbContext GetDbContext()
        {
            var scope = Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }
    }
}

/// <summary>
/// Handler de autenticação simples para testes que sempre autentica com sucesso
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                          ILoggerFactory logger, 
                          UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim("UserId", "1"),
            new Claim("SecurityStamp", "test-security-stamp")
        };

        var identity = new ClaimsIdentity(claims, "NoAuth");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "NoAuth");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
