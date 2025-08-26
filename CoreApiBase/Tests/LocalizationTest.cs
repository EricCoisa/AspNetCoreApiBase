using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using CoreApiBase.Resources;
using System.Globalization;

namespace CoreApiBase.Tests
{
    public class LocalizationTest
    {
        public static void TestLocalization()
        {
            // Test if we can access the localized strings
            var services = new ServiceCollection();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            var serviceProvider = services.BuildServiceProvider();
            var localizer = serviceProvider.GetService<IStringLocalizer<SharedResource>>();
            
            if (localizer != null)
            {
                // Test default culture (English)
                CultureInfo.CurrentUICulture = new CultureInfo("en-US");
                Console.WriteLine($"EN - UserNotFound: {localizer["UserNotFound"]}");
                Console.WriteLine($"EN - LoginFailed: {localizer["LoginFailed", "test error"]}");
                
                // Test Portuguese
                CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
                Console.WriteLine($"PT - UserNotFound: {localizer["UserNotFound"]}");
                Console.WriteLine($"PT - LoginFailed: {localizer["LoginFailed", "erro de teste"]}");
                
                // Test Spanish
                CultureInfo.CurrentUICulture = new CultureInfo("es-ES");
                Console.WriteLine($"ES - UserNotFound: {localizer["UserNotFound"]}");
                Console.WriteLine($"ES - LoginFailed: {localizer["LoginFailed", "error de prueba"]}");
            }
            else
            {
                Console.WriteLine("❌ Localizer is null - DI not working");
            }
        }
    }
}
