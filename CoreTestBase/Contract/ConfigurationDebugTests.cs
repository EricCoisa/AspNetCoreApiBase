using CoreTestBase.Integration;
using FluentAssertions;
using CoreApiBase.Configurations;

namespace CoreTestBase.Contract;

[Collection("IntegrationTests")]
public class ConfigurationDebugTests : IntegrationTestBase
{
    public ConfigurationDebugTests(IntegrationWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public void TestConfiguration_ShouldLoadCorrectly()
    {
        // Arrange & Act
        using var scope = Factory.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        
        // Assert
        var jwtSecretKey = configuration["JwtSettings:SecretKey"];
        jwtSecretKey.Should().NotBeNullOrEmpty();
        jwtSecretKey.Should().Be("test-secret-key-for-integration-tests-must-be-long-enough-to-pass-validation");
        
        var jwtIssuer = configuration["JwtSettings:Issuer"];
        jwtIssuer.Should().Be("TestIssuer");
        
        var jwtAudience = configuration["JwtSettings:Audience"];
        jwtAudience.Should().Be("TestAudience");
        
        // Test também o JwtSettings object
        var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
        jwtSettings.Should().NotBeNull();
        jwtSettings.SecretKey.Should().NotBeNullOrEmpty();
        jwtSettings.Issuer.Should().Be("TestIssuer");
        jwtSettings.Audience.Should().Be("TestAudience");
    }
}
