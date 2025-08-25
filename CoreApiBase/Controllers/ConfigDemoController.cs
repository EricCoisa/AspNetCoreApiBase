using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CoreApiBase.Configurations;

namespace CoreApiBase.Controllers
{
    /// <summary>
    /// Controller para demonstrar o uso de IOptions e IOptionsSnapshot.
    /// Remove este controller em produção - é apenas para demonstração.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigDemoController : ControllerBase
    {
        private readonly IOptions<JwtSettings> _jwtOptions;
        private readonly IOptionsSnapshot<DatabaseSettings> _dbOptionsSnapshot;
        private readonly IOptions<CorsSettings> _corsOptions;
        private readonly ILogger<ConfigDemoController> _logger;

        public ConfigDemoController(
            IOptions<JwtSettings> jwtOptions,
            IOptionsSnapshot<DatabaseSettings> dbOptionsSnapshot,
            IOptions<CorsSettings> corsOptions,
            ILogger<ConfigDemoController> logger)
        {
            _jwtOptions = jwtOptions;
            _dbOptionsSnapshot = dbOptionsSnapshot;
            _corsOptions = corsOptions;
            _logger = logger;
        }

        /// <summary>
        /// Demonstra o uso de IOptions (singleton - valor não muda durante execução).
        /// </summary>
        /// <returns>Configurações JWT mascaradas</returns>
        [HttpGet("jwt")]
        public ActionResult GetJwtConfig()
        {
            var jwt = _jwtOptions.Value;
            
            return Ok(new
            {
                message = "Configurações JWT via IOptions<T> (singleton)",
                config = new
                {
                    issuer = jwt.Issuer,
                    audience = jwt.Audience,
                    secretKeyLength = jwt.SecretKey?.Length ?? 0,
                    secretKeyConfigured = !string.IsNullOrEmpty(jwt.SecretKey),
                    expiryMinutes = jwt.ExpiryMinutes,
                    validateLifetime = jwt.ValidateLifetime
                }
            });
        }

        /// <summary>
        /// Demonstra o uso de IOptionsSnapshot (scoped - pode mudar entre requests).
        /// </summary>
        /// <returns>Configurações do banco mascaradas</returns>
        [HttpGet("database")]
        public ActionResult GetDatabaseConfig()
        {
            var db = _dbOptionsSnapshot.Value;
            
            return Ok(new
            {
                message = "Configurações Database via IOptionsSnapshot<T> (scoped - pode mudar)",
                config = new
                {
                    connectionConfigured = !string.IsNullOrEmpty(db.ConnectionString),
                    connectionStringLength = db.ConnectionString?.Length ?? 0,
                    commandTimeout = db.CommandTimeout,
                    autoMigrate = db.AutoMigrate,
                    seedData = db.SeedData,
                    logLevel = db.LogLevel,
                    enableSensitiveDataLogging = db.EnableSensitiveDataLogging
                }
            });
        }

        /// <summary>
        /// Demonstra configurações CORS.
        /// </summary>
        /// <returns>Configurações CORS</returns>
        [HttpGet("cors")]
        public ActionResult GetCorsConfig()
        {
            var cors = _corsOptions.Value;
            
            return Ok(new
            {
                message = "Configurações CORS via IOptions<T>",
                config = new
                {
                    allowedOrigins = cors.AllowedOrigins,
                    allowedMethods = cors.AllowedMethods,
                    allowedHeaders = cors.AllowedHeaders,
                    allowCredentials = cors.AllowCredentials,
                    preflightMaxAge = cors.PreflightMaxAge,
                    exposedHeaders = cors.ExposedHeaders
                }
            });
        }

        /// <summary>
        /// Demonstra todas as configurações em um só endpoint.
        /// </summary>
        /// <returns>Resumo de todas as configurações</returns>
        [HttpGet("all")]
        public ActionResult GetAllConfigs()
        {
            return Ok(new
            {
                message = "Resumo de todas as configurações (valores sensíveis mascarados)",
                jwt = _jwtOptions.Value.GetConfigurationSummary(),
                database = _dbOptionsSnapshot.Value.GetConfigurationSummary(),
                cors = _corsOptions.Value.GetConfigurationSummary(),
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            });
        }

        /// <summary>
        /// Força uma validação manual das configurações.
        /// </summary>
        /// <returns>Resultado da validação</returns>
        [HttpGet("validate")]
        public ActionResult ValidateConfigs()
        {
            var results = new Dictionary<string, object>();

            try
            {
                // Validar JWT
                var jwtErrors = _jwtOptions.Value.GetValidationErrors();
                results["jwt"] = new
                {
                    valid = !jwtErrors.Any(),
                    errors = jwtErrors
                };

                // Validar Database
                var dbErrors = _dbOptionsSnapshot.Value.GetValidationErrors();
                results["database"] = new
                {
                    valid = !dbErrors.Any(),
                    errors = dbErrors
                };

                // Validar CORS
                var corsErrors = _corsOptions.Value.GetValidationErrors();
                results["cors"] = new
                {
                    valid = !corsErrors.Any(),
                    errors = corsErrors
                };

                var allValid = !jwtErrors.Any() && !dbErrors.Any() && !corsErrors.Any();

                return Ok(new
                {
                    message = allValid ? "Todas as configurações são válidas" : "Algumas configurações são inválidas",
                    allValid,
                    results,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante validação manual de configurações");
                return StatusCode(500, new { message = "Erro durante validação", error = ex.Message });
            }
        }
    }
}
