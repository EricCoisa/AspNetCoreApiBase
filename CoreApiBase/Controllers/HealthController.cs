using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CoreApiBase.Controllers
{
    /// <summary>
    /// Controller para expor health checks através do Swagger.
    /// Permite testar e documentar os health checks da aplicação.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Health Checks")]
    [AllowAnonymous] // Health checks não devem exigir autenticação
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        /// <summary>
        /// Verifica o status geral de saúde da aplicação.
        /// </summary>
        /// <returns>Status de saúde da aplicação</returns>
        /// <response code="200">Aplicação está saudável</response>
        /// <response code="503">Aplicação não está saudável</response>
        [HttpGet]
        [ProducesResponseType(typeof(HealthResponse), 200)]
        [ProducesResponseType(typeof(HealthResponse), 503)]
        public async Task<ActionResult<HealthResponse>> GetHealth()
        {
            try
            {
                var report = await _healthCheckService.CheckHealthAsync();
                
                var response = new HealthResponse
                {
                    Status = report.Status.ToString(),
                    Duration = report.TotalDuration,
                    Timestamp = DateTime.UtcNow
                };

                var statusCode = report.Status == HealthStatus.Healthy ? 200 : 503;
                return StatusCode(statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar health check");
                
                return StatusCode(503, new HealthResponse
                {
                    Status = "Unhealthy",
                    Duration = TimeSpan.Zero,
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Verifica o status detalhado das configurações da aplicação.
        /// </summary>
        /// <returns>Status detalhado das configurações</returns>
        /// <response code="200">Todas as configurações estão válidas</response>
        /// <response code="503">Algumas configurações estão inválidas</response>
        [HttpGet("config")]
        [ProducesResponseType(typeof(DetailedHealthResponse), 200)]
        [ProducesResponseType(typeof(DetailedHealthResponse), 503)]
        public async Task<ActionResult<DetailedHealthResponse>> GetConfigHealth()
        {
            try
            {
                var report = await _healthCheckService.CheckHealthAsync(
                    check => check.Tags.Contains("config"));

                var response = new DetailedHealthResponse
                {
                    Status = report.Status.ToString(),
                    Duration = report.TotalDuration,
                    Timestamp = DateTime.UtcNow,
                    Checks = report.Entries.ToDictionary(
                        entry => entry.Key,
                        entry => new HealthCheckDetail
                        {
                            Status = entry.Value.Status.ToString(),
                            Description = entry.Value.Description,
                            Duration = entry.Value.Duration,
                            Data = entry.Value.Data,
                            Exception = entry.Value.Exception?.Message,
                            Tags = entry.Value.Tags?.ToArray() ?? Array.Empty<string>()
                        })
                };

                var statusCode = report.Status == HealthStatus.Healthy ? 200 : 503;
                return StatusCode(statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar health check de configuração");
                
                return StatusCode(503, new DetailedHealthResponse
                {
                    Status = "Unhealthy",
                    Duration = TimeSpan.Zero,
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message,
                    Checks = new Dictionary<string, HealthCheckDetail>()
                });
            }
        }

        /// <summary>
        /// Verifica health checks específicos por tag.
        /// </summary>
        /// <param name="tag">Tag dos health checks a verificar (ex: "config", "database", "external")</param>
        /// <returns>Status dos health checks filtrados por tag</returns>
        /// <response code="200">Health checks da tag estão saudáveis</response>
        /// <response code="503">Alguns health checks da tag não estão saudáveis</response>
        [HttpGet("tag/{tag}")]
        [ProducesResponseType(typeof(DetailedHealthResponse), 200)]
        [ProducesResponseType(typeof(DetailedHealthResponse), 503)]
        public async Task<ActionResult<DetailedHealthResponse>> GetHealthByTag(string tag)
        {
            try
            {
                var report = await _healthCheckService.CheckHealthAsync(
                    check => check.Tags.Contains(tag));

                var response = new DetailedHealthResponse
                {
                    Status = report.Status.ToString(),
                    Duration = report.TotalDuration,
                    Timestamp = DateTime.UtcNow,
                    FilteredByTag = tag,
                    Checks = report.Entries.ToDictionary(
                        entry => entry.Key,
                        entry => new HealthCheckDetail
                        {
                            Status = entry.Value.Status.ToString(),
                            Description = entry.Value.Description,
                            Duration = entry.Value.Duration,
                            Data = entry.Value.Data,
                            Exception = entry.Value.Exception?.Message,
                            Tags = entry.Value.Tags?.ToArray() ?? Array.Empty<string>()
                        })
                };

                var statusCode = report.Status == HealthStatus.Healthy ? 200 : 503;
                return StatusCode(statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar health check por tag: {Tag}", tag);
                
                return StatusCode(503, new DetailedHealthResponse
                {
                    Status = "Unhealthy",
                    Duration = TimeSpan.Zero,
                    Timestamp = DateTime.UtcNow,
                    FilteredByTag = tag,
                    Error = ex.Message,
                    Checks = new Dictionary<string, HealthCheckDetail>()
                });
            }
        }

        /// <summary>
        /// Lista todas as tags de health checks disponíveis.
        /// </summary>
        /// <returns>Lista de tags disponíveis</returns>
        /// <response code="200">Lista de tags retornada com sucesso</response>
        [HttpGet("tags")]
        [ProducesResponseType(typeof(HealthTagsResponse), 200)]
        public async Task<ActionResult<HealthTagsResponse>> GetHealthTags()
        {
            try
            {
                var report = await _healthCheckService.CheckHealthAsync();
                
                var allTags = report.Entries
                    .SelectMany(entry => entry.Value.Tags ?? Array.Empty<string>())
                    .Distinct()
                    .OrderBy(tag => tag)
                    .ToArray();

                var response = new HealthTagsResponse
                {
                    Tags = allTags,
                    TotalChecks = report.Entries.Count,
                    Timestamp = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tags de health check");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    /// <summary>
    /// Resposta básica de health check.
    /// </summary>
    public class HealthResponse
    {
        /// <summary>
        /// Status da aplicação (Healthy, Degraded, Unhealthy).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Tempo total gasto na verificação.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Timestamp da verificação.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Mensagem de erro, se houver.
        /// </summary>
        public string? Error { get; set; }
    }

    /// <summary>
    /// Resposta detalhada de health check.
    /// </summary>
    public class DetailedHealthResponse : HealthResponse
    {
        /// <summary>
        /// Detalhes de cada health check individual.
        /// </summary>
        public Dictionary<string, HealthCheckDetail> Checks { get; set; } = new();

        /// <summary>
        /// Tag usada para filtrar os health checks, se aplicável.
        /// </summary>
        public string? FilteredByTag { get; set; }
    }

    /// <summary>
    /// Detalhes de um health check específico.
    /// </summary>
    public class HealthCheckDetail
    {
        /// <summary>
        /// Status do health check (Healthy, Degraded, Unhealthy).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do health check.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Tempo gasto na verificação.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Dados adicionais do health check.
        /// </summary>
        public IReadOnlyDictionary<string, object>? Data { get; set; }

        /// <summary>
        /// Mensagem de exceção, se houver.
        /// </summary>
        public string? Exception { get; set; }

        /// <summary>
        /// Tags associadas ao health check.
        /// </summary>
        public string[] Tags { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// Resposta com tags de health checks disponíveis.
    /// </summary>
    public class HealthTagsResponse
    {
        /// <summary>
        /// Lista de todas as tags disponíveis.
        /// </summary>
        public string[] Tags { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Número total de health checks registrados.
        /// </summary>
        public int TotalChecks { get; set; }

        /// <summary>
        /// Timestamp da consulta.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
