using Microsoft.Extensions.Configuration;

namespace CoreApiBase.Configurations
{
    /// <summary>
    /// Provider de configuração que carrega Docker Secrets de arquivos.
    /// 
    /// Funcionamento:
    /// - Verifica se o diretório /run/secrets existe (indica execução em container)
    /// - Lê arquivos de secrets conforme mapeamento configurado
    /// - Carrega valores como configurações da aplicação
    /// 
    /// Em ambiente Docker:
    /// - Secrets são montados como arquivos em /run/secrets/
    /// - Cada arquivo contém o valor do secret (sem quebra de linha final)
    /// - Provider mapeia nome do arquivo para chave de configuração
    /// </summary>
    public class DockerSecretsConfigurationProvider : ConfigurationProvider
    {
        private readonly DockerSecretsConfigurationSource _source;

        public DockerSecretsConfigurationProvider(DockerSecretsConfigurationSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Carrega secrets dos arquivos Docker e popula o dicionário de configuração
        /// </summary>
        public override void Load()
        {
            Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            // Verifica se estamos em um ambiente Docker (diretório /run/secrets existe)
            if (!Directory.Exists(_source.SecretsPath))
            {
                // Não estamos em container ou secrets não estão configurados
                return;
            }

            // Carrega cada secret mapeado
            foreach (var mapping in _source.SecretMappings)
            {
                var secretFileName = mapping.Key;
                var configurationKey = mapping.Value;
                var secretFilePath = Path.Combine(_source.SecretsPath, secretFileName);

                try
                {
                    if (File.Exists(secretFilePath))
                    {
                        // Lê o valor do secret (remove quebras de linha)
                        var secretValue = File.ReadAllText(secretFilePath).Trim();
                        
                        if (!string.IsNullOrEmpty(secretValue))
                        {
                            Data[configurationKey] = secretValue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!_source.IgnoreErrors)
                    {
                        throw new InvalidOperationException(
                            $"Erro ao carregar Docker Secret '{secretFileName}' de '{secretFilePath}': {ex.Message}", ex);
                    }
                    
                    // Log do erro seria útil aqui em uma implementação real
                    // Para agora, apenas ignora o erro se IgnoreErrors = true
                }
            }
        }
    }
}
