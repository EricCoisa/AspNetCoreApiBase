using System.ComponentModel.DataAnnotations;

namespace CoreApiBase.Configurations
{
    /// <summary>
    /// Configurações de CORS (Cross-Origin Resource Sharing) da aplicação.
    /// </summary>
    public class CorsSettings
    {
        public const string SectionName = "CorsSettings";

        /// <summary>
        /// Lista de origens permitidas para requisições CORS.
        /// Use "*" para permitir todas as origens (apenas para desenvolvimento).
        /// </summary>
        [Required(ErrorMessage = "AllowedOrigins é obrigatório")]
        [MinLength(1, ErrorMessage = "Deve ter pelo menos uma origem permitida")]
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Lista de métodos HTTP permitidos.
        /// Padrão: GET, POST, PUT, DELETE, OPTIONS.
        /// </summary>
        public string[] AllowedMethods { get; set; } = { "GET", "POST", "PUT", "DELETE", "OPTIONS" };

        /// <summary>
        /// Lista de headers permitidos.
        /// Use "*" para permitir todos os headers.
        /// </summary>
        public string[] AllowedHeaders { get; set; } = { "*" };

        /// <summary>
        /// Se deve permitir credenciais (cookies, headers de autorização).
        /// </summary>
        public bool AllowCredentials { get; set; } = true;

        /// <summary>
        /// Tempo de cache para preflight requests em segundos.
        /// Padrão: 3600 segundos (1 hora).
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "PreflightMaxAge deve ser >= 0")]
        public int PreflightMaxAge { get; set; } = 3600;

        /// <summary>
        /// Headers que o cliente pode acessar na resposta.
        /// </summary>
        public string[] ExposedHeaders { get; set; } = Array.Empty<string>();
    }
}
