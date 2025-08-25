using CoreApiBase.Configurations;

namespace CoreApiBase.Extensions
{
    /// <summary>
    /// Extensões para configuração de Docker Secrets no IConfigurationBuilder
    /// </summary>
    public static class DockerSecretsConfigurationExtensions
    {
        /// <summary>
        /// Adiciona suporte a Docker Secrets na configuração da aplicação.
        /// 
        /// Uso:
        /// builder.Configuration.AddDockerSecrets(secrets =>
        /// {
        ///     secrets.AddSecret("jwt_secret", "Jwt:Secret");
        ///     secrets.AddSecret("db_password", "ConnectionStrings:Password");
        /// });
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="configureSecrets">Ação para configurar mapeamentos de secrets</param>
        /// <returns>Configuration builder para chaining</returns>
        public static IConfigurationBuilder AddDockerSecrets(
            this IConfigurationBuilder builder,
            Action<DockerSecretsConfiguration>? configureSecrets = null)
        {
            var secretsConfig = new DockerSecretsConfiguration();
            configureSecrets?.Invoke(secretsConfig);

            var source = new DockerSecretsConfigurationSource
            {
                SecretsPath = secretsConfig.SecretsPath,
                SecretMappings = secretsConfig.SecretMappings,
                IgnoreErrors = secretsConfig.IgnoreErrors
            };

            return builder.Add(source);
        }

        /// <summary>
        /// Adiciona Docker Secrets com configuração manual da fonte
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="source">Fonte de configuração Docker Secrets</param>
        /// <returns>Configuration builder para chaining</returns>
        public static IConfigurationBuilder AddDockerSecrets(
            this IConfigurationBuilder builder,
            DockerSecretsConfigurationSource source)
        {
            return builder.Add(source);
        }
    }

    /// <summary>
    /// Classe auxiliar para configuração fluente de Docker Secrets
    /// </summary>
    public class DockerSecretsConfiguration
    {
        public string SecretsPath { get; set; } = "/run/secrets";
        public Dictionary<string, string> SecretMappings { get; set; } = new();
        public bool IgnoreErrors { get; set; } = true;

        /// <summary>
        /// Adiciona um mapeamento de secret
        /// </summary>
        /// <param name="secretFileName">Nome do arquivo do secret (sem caminho)</param>
        /// <param name="configurationKey">Chave de configuração onde o valor será disponibilizado</param>
        /// <returns>Esta instância para chaining</returns>
        public DockerSecretsConfiguration AddSecret(string secretFileName, string configurationKey)
        {
            SecretMappings[secretFileName] = configurationKey;
            return this;
        }

        /// <summary>
        /// Define o caminho base dos secrets (padrão: /run/secrets)
        /// </summary>
        /// <param name="path">Caminho base</param>
        /// <returns>Esta instância para chaining</returns>
        public DockerSecretsConfiguration WithSecretsPath(string path)
        {
            SecretsPath = path;
            return this;
        }

        /// <summary>
        /// Define se erros de leitura devem ser ignorados
        /// </summary>
        /// <param name="ignore">True para ignorar erros</param>
        /// <returns>Esta instância para chaining</returns>
        public DockerSecretsConfiguration WithIgnoreErrors(bool ignore = true)
        {
            IgnoreErrors = ignore;
            return this;
        }
    }
}
