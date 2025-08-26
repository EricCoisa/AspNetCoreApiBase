using CoreApiBase.Configurations;
using CoreApiBase.Extensions;
using CoreApiBase.Middlewares;
using CoreDomainBase.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CoreDomainBase.Enums;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Configuração customizada de secrets e configurações na ordem de prioridade:
// 1. appsettings.json (já carregado automaticamente)
// 2. appsettings.{Environment}.json (já carregado automaticamente)
// 3. User Secrets (apenas em Development)
// 4. Environment Variables (já carregado automaticamente)
// 5. Docker Secrets (quando em container)

// Limpa configurações padrão para controlar a ordem
builder.Configuration.Sources.Clear();

// 1. appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 2. appsettings.{Environment}.json (Release usa Production)
var environmentName = builder.Environment.EnvironmentName == "Release" ? "Production" : builder.Environment.EnvironmentName;
builder.Configuration.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

// 3. User Secrets (apenas em Development)
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// 4. Environment Variables
builder.Configuration.AddEnvironmentVariables();

// Mapear variáveis de ambiente para configurações internas (Production/Release)
if (builder.Environment.IsProduction() || builder.Environment.EnvironmentName == "Release")
{
    var memoryConfig = new Dictionary<string, string?>();
    
    // Mapear JWT Settings
    var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
    if (!string.IsNullOrEmpty(jwtSecretKey))
        memoryConfig["JwtSettings:SecretKey"] = jwtSecretKey;
    
    var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
    if (!string.IsNullOrEmpty(jwtIssuer))
        memoryConfig["JwtSettings:Issuer"] = jwtIssuer;
    
    var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
    if (!string.IsNullOrEmpty(jwtAudience))
        memoryConfig["JwtSettings:Audience"] = jwtAudience;
    
    // Mapear Connection String
    var dbConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
    if (!string.IsNullOrEmpty(dbConnectionString))
        memoryConfig["ConnectionStrings:DefaultConnection"] = dbConnectionString;
    
    // Mapear CORS Settings
    var corsOriginsEnv = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
    if (!string.IsNullOrEmpty(corsOriginsEnv))
    {
        var corsOrigins = corsOriginsEnv.Split(',');
        for (int i = 0; i < corsOrigins.Length; i++)
        {
            memoryConfig[$"CorsSettings:AllowedOrigins:{i}"] = corsOrigins[i].Trim();
        }
    }
    
    builder.Configuration.AddInMemoryCollection(memoryConfig);
}

// 5. Docker Secrets (quando em container/produção)
builder.Configuration.AddDockerSecrets(secrets =>
{
    // Mapeamento de secrets para chaves de configuração
    // Adicione novos secrets aqui conforme necessário
    secrets.AddSecret("jwt_secret", "JwtSettings:SecretKey")
           .AddSecret("jwt_issuer", "JwtSettings:Issuer")
           .AddSecret("jwt_audience", "JwtSettings:Audience")
           .AddSecret("db_connection", "ConnectionStrings:DefaultConnection")
           .AddSecret("cors_origins", "CorsSettings:AllowedOrigins")
           .WithIgnoreErrors(true); // Ignora erros se secrets não existirem
});

// ⚠️ VERIFICAÇÃO CRÍTICA: Validar configurações obrigatórias APÓS configuração completa
// Se as configurações estão faltando, mostra página de instruções e para a execução
ConfigurationSetupHelper.ValidateRequiredConfigurationsOrShowSetupPage(builder.Configuration, builder.Environment);

// 🔍 DEBUG: Verificar se as variáveis foram mapeadas corretamente
if (builder.Environment.IsProduction() || builder.Environment.EnvironmentName == "Release")
{
    Console.WriteLine($"[DEBUG] Environment: {builder.Environment.EnvironmentName}");
    Console.WriteLine($"[DEBUG] JWT_SECRET_KEY env var: {Environment.GetEnvironmentVariable("JWT_SECRET_KEY")?[..20]}...");
    Console.WriteLine($"[DEBUG] JwtSettings:SecretKey config: {builder.Configuration["JwtSettings:SecretKey"]?[..20]}...");
    Console.WriteLine($"[DEBUG] DATABASE_CONNECTION_STRING env var: {Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")}");
    Console.WriteLine($"[DEBUG] ConnectionStrings:DefaultConnection config: {builder.Configuration["ConnectionStrings:DefaultConnection"]}");
}

// Configure JWT Settings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

// Configurar e validar todas as configurações usando Options Pattern
// Isso validará configurações obrigatórias na inicialização
builder.Services.AddAndValidateConfigurations(builder.Configuration);

// Configure CORS
var corsSettings = builder.Configuration.GetSection("CorsSettings");
var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProjectDependencies();
builder.Services.AddProjectAutoMapper();

// Adicionar health checks customizados
builder.Services.AddCustomHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"), 
        b => b.MigrationsAssembly("CoreApiBase")));

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var supportedCultures = new[] { "en-US", "pt-BR", "es-ES" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
    options.SupportedCultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
    
    // Configure culture providers: QueryString first, then Accept-Language
    options.RequestCultureProviders.Clear();
    options.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.QueryStringRequestCultureProvider());
    options.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider());
});

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey))
    };
});

// Configure Authorization with Claims Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policy.AdminOnly.ToString(), policy => policy.RequireRole(Roles.Admin.ToString()));
    options.AddPolicy(Policy.UserOrAdmin.ToString(), policy => policy.RequireRole(Roles.User.ToString(), Roles.Admin.ToString()));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Core API Base",
        Version = "v1",
        Description = "API base com autenticação JWT, validação de configurações e health checks",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Core API Team",
            Email = "admin@coreapi.com"
        }
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Incluir comentários XML para documentação
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Agrupar endpoints por tags
    options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    options.DocInclusionPredicate((name, api) => true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Request Localization
var localizationOptions = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();

// Security stamp validation middleware (after auth)
app.UseMiddleware<SecurityStampValidationMiddleware>();

// Database role authorization middleware (replaces token role with DB role)
app.UseMiddleware<DatabaseRoleAuthorizationMiddleware>();

app.UseAuthorization();

// Optional auth logging middleware
app.UseMiddleware<AuthLoggingMiddleware>();

// Mapear health checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/config", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("config"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new
                {
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    data = entry.Value.Data
                }
            ),
            duration = report.TotalDuration
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

// Auto migrate database if enabled
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var dbSettings = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseSettings>>().Value;
    
    if (dbSettings.AutoMigrate)
    {
        try
        {
            context.Database.Migrate();
            Console.WriteLine("[INFO] Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to apply database migrations: {ex.Message}");
        }
    }
}

app.MapControllers();

app.Run();
