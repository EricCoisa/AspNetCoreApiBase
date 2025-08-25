using Microsoft.Extensions.Configuration;

namespace CoreApiBase.Configurations
{
    /// <summary>
    /// Fonte de configuração para Docker Secrets.
    /// Carrega secrets de arquivos localizados em /run/secrets/ quando em containers Docker.
    /// 
    /// Para adicionar novos secrets:
    /// 1. No docker-compose.yml:
    ///    secrets:
    ///      jwt_secret:
    ///        file: ./secrets/jwt_secret.txt
    ///    services:
    ///      app:
    ///        secrets:
    ///          - jwt_secret
    /// 
    /// 2. Crie o arquivo de secret: ./secrets/jwt_secret.txt com o valor
    /// 3. O secret estará disponível como Configuration["Jwt:Secret"] (usando mapeamento)
    /// </summary>
    public class DockerSecretsConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Caminho base onde Docker monta os secrets (padrão: /run/secrets)
        /// </summary>
        public string SecretsPath { get; set; } = "/run/secrets";

        /// <summary>
        /// Mapeamento de nomes de arquivos de secrets para chaves de configuração.
        /// Exemplo: { "jwt_secret", "Jwt:Secret" } mapeia o arquivo jwt_secret para a chave Jwt:Secret
        /// </summary>
        public Dictionary<string, string> SecretMappings { get; set; } = new();

        /// <summary>
        /// Se true, ignora erros de leitura de arquivos (útil quando nem todos os secrets são obrigatórios)
        /// </summary>
        public bool IgnoreErrors { get; set; } = true;

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DockerSecretsConfigurationProvider(this);
        }
    }
}
