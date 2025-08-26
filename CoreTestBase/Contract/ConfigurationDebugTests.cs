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
        
        // Assert - validar que as configurações JWT estão presentes e válidas
        var jwtSecretKey = configuration["JwtSettings:SecretKey"];
        jwtSecretKey.Should().NotBeNullOrEmpty();
        jwtSecretKey.Should().HaveLength(64, "JWT secret key should be 64 characters long for security");
        
        var jwtIssuer = configuration["JwtSettings:Issuer"];
        jwtIssuer.Should().Be("https://localhost:5001");
        
        var jwtAudience = configuration["JwtSettings:Audience"];
        jwtAudience.Should().Be("https://localhost:5001");
        
        // Test também o JwtSettings object
        var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
        jwtSettings.Should().NotBeNull();
        jwtSettings.SecretKey.Should().NotBeNullOrEmpty();
        jwtSettings.SecretKey.Should().HaveLength(64, "JWT settings secret key should be 64 characters long");
        jwtSettings.Issuer.Should().Be("https://localhost:5001");
        jwtSettings.Audience.Should().Be("https://localhost:5001");
        jwtSettings.ExpiryMinutes.Should().Be(60);
    }
}
