using System.ComponentModel.DataAnnotations;

namespace CoreApiBase.Configurations
{
    /// <summary>
    /// Configurações do banco de dados da aplicação.
    /// </summary>
    public class DatabaseSettings
    {
        public const string SectionName = "DatabaseSettings";

        /// <summary>
        /// String de conexão com o banco de dados.
        /// </summary>
        [Required(ErrorMessage = "Connection String é obrigatória")]
        [MinLength(10, ErrorMessage = "Connection String deve ter pelo menos 10 caracteres")]
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Timeout para comandos do banco em segundos.
        /// Padrão: 30 segundos.
        /// </summary>
        [Range(1, 300, ErrorMessage = "CommandTimeout deve estar entre 1 e 300 segundos")]
        public int CommandTimeout { get; set; } = 30;

        /// <summary>
        /// Se deve aplicar migrações automaticamente na inicialização.
        /// </summary>
        public bool AutoMigrate { get; set; } = false;

        /// <summary>
        /// Se deve executar seed de dados na inicialização.
        /// </summary>
        public bool SeedData { get; set; } = false;

        /// <summary>
        /// Nível de log do Entity Framework.
        /// </summary>
        public string LogLevel { get; set; } = "Warning";

        /// <summary>
        /// Se deve mostrar dados sensíveis nos logs (apenas para desenvolvimento).
        /// </summary>
        public bool EnableSensitiveDataLogging { get; set; } = false;
    }
}
