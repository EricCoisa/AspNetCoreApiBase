using System.ComponentModel.DataAnnotations;

namespace CoreApiBase.Configurations
{
    /// <summary>
    /// Configurações do JWT (JSON Web Token) para autenticação.
    /// </summary>
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        /// <summary>
        /// Chave secreta para assinar e validar tokens JWT.
        /// Deve ter pelo menos 256 bits (32 caracteres) para HS256.
        /// </summary>
        [Required(ErrorMessage = "JWT SecretKey é obrigatório")]
        [MinLength(32, ErrorMessage = "JWT SecretKey deve ter pelo menos 32 caracteres para segurança")]
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Emissor do token (quem criou o token).
        /// Normalmente é a URL da aplicação.
        /// </summary>
        [Required(ErrorMessage = "JWT Issuer é obrigatório")]
        [Url(ErrorMessage = "JWT Issuer deve ser uma URL válida")]
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Audiência do token (para quem o token é destinado).
        /// Normalmente é a URL da aplicação ou cliente.
        /// </summary>
        [Required(ErrorMessage = "JWT Audience é obrigatório")]
        [Url(ErrorMessage = "JWT Audience deve ser uma URL válida")]
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Tempo de expiração do token em minutos.
        /// Padrão: 60 minutos.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "JWT ExpiryMinutes deve ser maior que 0")]
        public int ExpiryMinutes { get; set; } = 60;

        /// <summary>
        /// Se deve validar o tempo de vida do token.
        /// </summary>
        public bool ValidateLifetime { get; set; } = true;

        /// <summary>
        /// Se deve validar a chave de assinatura.
        /// </summary>
        public bool ValidateIssuerSigningKey { get; set; } = true;
    }
}
