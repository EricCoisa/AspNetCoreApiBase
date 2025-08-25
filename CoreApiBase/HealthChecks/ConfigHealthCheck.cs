using CoreApiBase.Configurations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace CoreApiBase.HealthChecks
{
    /// <summary>
    /// Health check customizado para verificar se as configurações essenciais estão válidas.
    /// 
    /// Verifica:
    /// - JwtSettings: SecretKey, Issuer, Audience
    /// - DatabaseSettings: ConnectionString
    /// - CorsSettings: AllowedOrigins
    /// 
    /// Para adicionar nova verificação:
    /// 1. Injete o IOptions de NovaConfig no construtor
    /// 2. Adicione a validação no método CheckHealthAsync
    /// 3. Registre a nova configuração no Program.cs
    /// </summary>
    public class ConfigHealthCheck : IHealthCheck
    {
        private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly IOptions<DatabaseSettings> _databaseSettings;
        private readonly IOptions<CorsSettings> _corsSettings;
        private readonly ILogger<ConfigHealthCheck> _logger;

        public ConfigHealthCheck(
            IOptions<JwtSettings> jwtSettings,
            IOptions<DatabaseSettings> databaseSettings,
            IOptions<CorsSettings> corsSettings,
            ILogger<ConfigHealthCheck> logger)
        {
            _jwtSettings = jwtSettings;
            _databaseSettings = databaseSettings;
            _corsSettings = corsSettings;
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var healthData = new Dictionary<string, object>();
                var errors = new List<string>();

                // Verificar JwtSettings
                CheckJwtSettings(healthData, errors);

                // Verificar DatabaseSettings
                CheckDatabaseSettings(healthData, errors);

                // Verificar CorsSettings
                CheckCorsSettings(healthData, errors);

                // Determinar resultado do health check
                if (errors.Any())
                {
                    var errorMessage = $"Configurações inválidas encontradas: {string.Join("; ", errors)}";
                    _logger.LogError("Health check de configuração falhou: {Errors}", string.Join(", ", errors));
                    
                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        errorMessage, 
                        data: healthData));
                }

                _logger.LogDebug("Health check de configuração passou com sucesso");
                
                return Task.FromResult(HealthCheckResult.Healthy(
                    "Todas as configurações essenciais estão válidas", 
                    data: healthData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado durante health check de configuração");
                
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Erro inesperado: {ex.Message}",
                    ex));
            }
        }

        /// <summary>
        /// Verifica as configurações JWT.
        /// </summary>
        private void CheckJwtSettings(Dictionary<string, object> healthData, List<string> errors)
        {
            try
            {
                var jwt = _jwtSettings.Value;
                var jwtErrors = jwt.GetValidationErrors();
                
                if (jwtErrors.Any())
                {
                    errors.AddRange(jwtErrors.Select(e => $"JWT: {e}"));
                }

                // Verificações adicionais específicas
                if (!string.IsNullOrEmpty(jwt.SecretKey) && jwt.SecretKey.Length < 32)
                {
                    errors.Add("JWT: SecretKey muito curta (mínimo 32 caracteres)");
                }

                healthData["jwt_config"] = jwt.GetConfigurationSummary();
                healthData["jwt_valid"] = !jwtErrors.Any();
            }
            catch (Exception ex)
            {
                errors.Add($"JWT: Erro ao acessar configurações - {ex.Message}");
                healthData["jwt_config"] = "ERROR";
                healthData["jwt_valid"] = false;
            }
        }

        /// <summary>
        /// Verifica as configurações do banco de dados.
        /// </summary>
        private void CheckDatabaseSettings(Dictionary<string, object> healthData, List<string> errors)
        {
            try
            {
                var db = _databaseSettings.Value;
                var dbErrors = db.GetValidationErrors();
                
                if (dbErrors.Any())
                {
                    errors.AddRange(dbErrors.Select(e => $"Database: {e}"));
                }

                healthData["database_config"] = db.GetConfigurationSummary();
                healthData["database_valid"] = !dbErrors.Any();
            }
            catch (Exception ex)
            {
                errors.Add($"Database: Erro ao acessar configurações - {ex.Message}");
                healthData["database_config"] = "ERROR";
                healthData["database_valid"] = false;
            }
        }

        /// <summary>
        /// Verifica as configurações CORS.
        /// </summary>
        private void CheckCorsSettings(Dictionary<string, object> healthData, List<string> errors)
        {
            try
            {
                var cors = _corsSettings.Value;
                var corsErrors = cors.GetValidationErrors();
                
                if (corsErrors.Any())
                {
                    errors.AddRange(corsErrors.Select(e => $"CORS: {e}"));
                }

                // Verificação adicional: avisar sobre configuração insegura
                if (cors.AllowedOrigins.Contains("*") && cors.AllowCredentials)
                {
                    errors.Add("CORS: Configuração insegura - AllowCredentials=true com AllowedOrigins=*");
                }

                healthData["cors_config"] = cors.GetConfigurationSummary();
                healthData["cors_valid"] = !corsErrors.Any();
            }
            catch (Exception ex)
            {
                errors.Add($"CORS: Erro ao acessar configurações - {ex.Message}");
                healthData["cors_config"] = "ERROR";
                healthData["cors_valid"] = false;
            }
        }
    }
}
