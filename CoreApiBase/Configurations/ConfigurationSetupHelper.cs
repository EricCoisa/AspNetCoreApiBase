using System.Text;

namespace CoreApiBase.Configurations;

public static class ConfigurationSetupHelper
{
    public static void ValidateRequiredConfigurationsOrShowSetupPage(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var missingConfigurations = new List<string>();

        // Verificar JWT Settings
        var jwtSecretKey = configuration["JwtSettings:SecretKey"] ?? 
                          configuration["JWT_SECRET_KEY"];
        if (string.IsNullOrEmpty(jwtSecretKey))
        {
            missingConfigurations.Add("JWT SecretKey é obrigatório");
        }

        // Verificar Connection String
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                              configuration["DatabaseSettings:ConnectionString"] ??
                              configuration["DATABASE_CONNECTION_STRING"];
        if (string.IsNullOrEmpty(connectionString))
        {
            missingConfigurations.Add("Connection String é obrigatória");
        }

        // Se há configurações faltando, mostrar instruções e parar execução
        if (missingConfigurations.Any())
        {
            HandleMissingConfigurations(missingConfigurations, environment);
            
            // Instruções específicas baseadas no ambiente
            var setupCommand = environment.IsDevelopment() ? "development" : 
                              environment.IsProduction() || environment.EnvironmentName == "Release" ? "release" : 
                              "development";
            
            var visualStudioInstructions = "";
            if (environment.EnvironmentName == "Release" || environment.IsProduction())
            {
                visualStudioInstructions = Environment.NewLine + 
                    "💡 Para Visual Studio Release: Use o perfil 'http (Release)' ou 'https (Release)' na lista de perfis." + Environment.NewLine +
                    "   Ou execute: start setup-configuration.bat release";
            }
            
            // Forçar parada da aplicação com mensagem clara
            throw new InvalidOperationException(
                $"🔧 CONFIGURAÇÃO NECESSÁRIA: Execute 'start setup-configuration.bat {setupCommand}' no diretório raiz do projeto. " +
                "Arquivo 'configuracao-necessaria.html' criado com instruções detalhadas." + visualStudioInstructions);
        }
    }

    private static void HandleMissingConfigurations(List<string> missingConfigurations, IWebHostEnvironment environment)
    {
        try
        {
            // Caminho do arquivo HTML fixo
            var htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "configRequired.html");

            // Mostrar mensagem no console (se disponível)
            ShowConsoleMessage(missingConfigurations, htmlFilePath);

            // Tentar abrir o arquivo HTML no navegador
            TryOpenInBrowser(htmlFilePath);

            // Log para Debug/Output do Visual Studio
            LogToDebugOutput(missingConfigurations, htmlFilePath);
        }
        catch (Exception ex)
        {
            // Fallback: apenas mostrar no Debug se algo falhar
            System.Diagnostics.Debug.WriteLine($"⚠️ Erro ao abrir arquivo de instruções: {ex.Message}");
            System.Diagnostics.Debug.WriteLine("🔧 EXECUTE: start setup-configuration.bat development");
        }
    }

    private static void ShowConsoleMessage(List<string> missingConfigurations, string htmlFilePath)
    {
        try
        {
            Console.WriteLine();
            Console.WriteLine("================================================================================");
            Console.WriteLine("🔧 CONFIGURAÇÃO NECESSÁRIA - ASP.NET Core API Base");
            Console.WriteLine("================================================================================");
            Console.WriteLine();
            
            Console.WriteLine("❌ Configurações obrigatórias estão faltando:");
            foreach (var missing in missingConfigurations)
            {
                Console.WriteLine($"   • {missing}");
            }
            
            Console.WriteLine();
            Console.WriteLine("✅ SOLUÇÃO: Execute o script de configuração");
            Console.WriteLine();
            Console.WriteLine("Passos:");
            Console.WriteLine("   1. Abra um terminal no diretório raiz do projeto");
            Console.WriteLine("   2. Execute: start setup-configuration.bat development");
            Console.WriteLine("   3. Execute o projeto novamente");
            Console.WriteLine();
            Console.WriteLine($"📄 Instruções detalhadas salvas em: {Path.GetFileName(htmlFilePath)}");
            Console.WriteLine("================================================================================");
        }
        catch (IOException)
        {
            // Ignore se console não estiver disponível
        }
    }

    private static void TryOpenInBrowser(string htmlFilePath)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = htmlFilePath,
                    UseShellExecute = true
                });
            }
            else if (OperatingSystem.IsLinux())
            {
                System.Diagnostics.Process.Start("xdg-open", htmlFilePath);
            }
            else if (OperatingSystem.IsMacOS())
            {
                System.Diagnostics.Process.Start("open", htmlFilePath);
            }
        }
        catch
        {
            // Ignore se não conseguir abrir no navegador
        }
    }

    private static void LogToDebugOutput(List<string> missingConfigurations, string htmlFilePath)
    {
        System.Diagnostics.Debug.WriteLine("================================================================================");
        System.Diagnostics.Debug.WriteLine("🔧 CONFIGURAÇÃO NECESSÁRIA - ASP.NET Core API Base");
        System.Diagnostics.Debug.WriteLine("================================================================================");
        System.Diagnostics.Debug.WriteLine("");
        System.Diagnostics.Debug.WriteLine("❌ Configurações obrigatórias estão faltando:");
        foreach (var missing in missingConfigurations)
        {
            System.Diagnostics.Debug.WriteLine($"   • {missing}");
        }
        System.Diagnostics.Debug.WriteLine("");
        System.Diagnostics.Debug.WriteLine("✅ SOLUÇÃO: Execute o script de configuração");
        System.Diagnostics.Debug.WriteLine("   1. Abra um terminal no diretório raiz do projeto");
        System.Diagnostics.Debug.WriteLine("   2. Execute: start setup-configuration.bat development");
        System.Diagnostics.Debug.WriteLine("   3. Execute o projeto novamente");
        System.Diagnostics.Debug.WriteLine("");
        System.Diagnostics.Debug.WriteLine($"📄 Instruções detalhadas: {htmlFilePath}");
        System.Diagnostics.Debug.WriteLine("================================================================================");
    }
}

