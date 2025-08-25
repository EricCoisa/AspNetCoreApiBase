using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CoreApiBase.Configurations
{
    /// <summary>
    /// Extensões para validação de configurações usando Data Annotations.
    /// </summary>
    public static class ConfigurationValidationExtensions
    {
        /// <summary>
        /// Valida uma configuração usando Data Annotations e lança exceção se inválida.
        /// </summary>
        /// <typeparam name="T">Tipo da configuração</typeparam>
        /// <param name="configuration">Instância da configuração</param>
        /// <param name="sectionName">Nome da seção para mensagens de erro</param>
        /// <exception cref="InvalidOperationException">Quando a configuração é inválida</exception>
        public static void ValidateConfiguration<T>(this T configuration, string sectionName) 
            where T : class
        {
            var validationContext = new ValidationContext(configuration);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(configuration, validationContext, validationResults, true);

            if (!isValid)
            {
                var errors = validationResults.Select(vr => $"  - {vr.ErrorMessage}");
                var errorMessage = $"Configuração inválida na seção '{sectionName}':\n{string.Join("\n", errors)}";
                
                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Valida todas as propriedades obrigatórias de uma configuração.
        /// </summary>
        /// <typeparam name="T">Tipo da configuração</typeparam>
        /// <param name="configuration">Instância da configuração</param>
        /// <returns>Lista de propriedades que falharam na validação</returns>
        public static List<string> GetValidationErrors<T>(this T configuration) where T : class
        {
            var validationContext = new ValidationContext(configuration);
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateObject(configuration, validationContext, validationResults, true);

            return validationResults.Select(vr => vr.ErrorMessage ?? "Erro de validação desconhecido").ToList();
        }

        /// <summary>
        /// Verifica se uma configuração é válida sem lançar exceção.
        /// </summary>
        /// <typeparam name="T">Tipo da configuração</typeparam>
        /// <param name="configuration">Instância da configuração</param>
        /// <returns>True se válida, False caso contrário</returns>
        public static bool IsValid<T>(this T configuration) where T : class
        {
            var validationContext = new ValidationContext(configuration);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(configuration, validationContext, validationResults, true);
        }

        /// <summary>
        /// Obtém informações resumidas sobre uma configuração para health checks.
        /// </summary>
        /// <typeparam name="T">Tipo da configuração</typeparam>
        /// <param name="configuration">Instância da configuração</param>
        /// <returns>Dicionário com informações da configuração (sem valores sensíveis)</returns>
        public static Dictionary<string, object> GetConfigurationSummary<T>(this T configuration) where T : class
        {
            var summary = new Dictionary<string, object>();
            var type = typeof(T);
            
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(configuration);
                
                // Mascarar valores sensíveis
                if (IsSensitiveProperty(property.Name))
                {
                    summary[property.Name] = value != null ? "***CONFIGURED***" : "***NOT SET***";
                }
                else
                {
                    summary[property.Name] = value ?? "NULL";
                }
            }

            return summary;
        }

        /// <summary>
        /// Determina se uma propriedade contém dados sensíveis que devem ser mascarados.
        /// </summary>
        /// <param name="propertyName">Nome da propriedade</param>
        /// <returns>True se for sensível</returns>
        private static bool IsSensitiveProperty(string propertyName)
        {
            var sensitiveKeywords = new[] { "secret", "key", "password", "token", "connection" };
            return sensitiveKeywords.Any(keyword => 
                propertyName.ToLowerInvariant().Contains(keyword));
        }
    }
}
