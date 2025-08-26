using CoreDomainBase.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CoreDomainBase.Enums;
using CoreTestBase.Integration;

namespace CoreTestBase.Contract
{
    /// <summary>
    /// Testes de contrato (snapshot) para validar que as respostas da API não mudam inesperadamente
    /// </summary>
    public class UserContractTests : IntegrationTestBase
    {
        public UserContractTests(IntegrationWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAllUsers_WithAdminAuth_ShouldMatchSnapshot()
        {
            // Arrange
            await Factory.InitializeDatabaseAsync();
            var adminToken = GenerateJwtToken(1, "admin", Roles.Admin);
            Client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            // Act
            var response = await Client.GetAsync("/api/user/GetAll");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            
            // Snapshot test - garante que a estrutura JSON não mude inesperadamente
            Snapshot.Match(content, "GetAllUsers_AdminAuth");
        }

        [Fact]
        public async Task GetUserById_WithValidId_ShouldMatchSnapshot()
        {
            // Arrange
            await Factory.InitializeDatabaseAsync();
            var userToken = GenerateJwtToken(1, "testuser1", Roles.User);
            Client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);

            // Act
            var response = await Client.GetAsync("/api/user/1");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            
            // Snapshot test
            Snapshot.Match(content, "GetUserById_ValidId");
        }

        [Fact]
        public async Task GetUserById_WithInvalidId_ShouldMatchSnapshot()
        {
            // Arrange
            await Factory.InitializeDatabaseAsync();
            var adminToken = GenerateJwtToken(1, "admin", Roles.Admin);
            Client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            // Act
            var response = await Client.GetAsync("/api/user/999");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().NotBeNullOrEmpty();
            
            // Snapshot test para resposta de erro
            Snapshot.Match(content, "GetUserById_InvalidId");
        }

        [Fact]
        public async Task GetAllUsers_WithoutAuth_ShouldReturnUnauthorized()
        {
            // Arrange
            await Factory.InitializeDatabaseAsync();
            // Não configurar authorization header

            // Act
            var response = await Client.GetAsync("/api/user/GetAll");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            
            // Snapshot test para resposta de não autorizado
            Snapshot.Match(content, "GetAllUsers_Unauthorized");
        }

        [Fact]
        public async Task GetUserById_WithUserAuth_AccessingOwnData_ShouldMatchSnapshot()
        {
            // Arrange
            await Factory.InitializeDatabaseAsync();
            var userToken = GenerateJwtToken(2, "testuser2", Roles.User);
            Client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);

            // Act
            var response = await Client.GetAsync("/api/user/2");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            
            // Snapshot test
            Snapshot.Match(content, "GetUserById_UserAuth_OwnData");
        }

        [Fact]
        public async Task GetUserById_WithUserAuth_AccessingOtherData_ShouldReturnForbidden()
        {
            // Arrange
            await Factory.InitializeDatabaseAsync();
            var userToken = GenerateJwtToken(1, "testuser1", Roles.User);
            Client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);

            // Act - Tentar acessar dados de outro usuário
            var response = await Client.GetAsync("/api/user/2");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            
            // Snapshot test para resposta proibida
            Snapshot.Match(content, "GetUserById_UserAuth_OtherData_Forbidden");
        }

        [Fact]
        public async Task CreateUser_WithValidData_ShouldMatchSnapshot()
        {
            // Arrange
            await Factory.InitializeDatabaseAsync();
            var adminToken = GenerateJwtToken(1, "admin", Roles.Admin);
            Client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            var newUser = new
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Name = "New User",
                Role = "User"
            };

            var json = JsonSerializer.Serialize(newUser);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/user", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            responseContent.Should().NotBeNullOrEmpty();
            
            // Normalizar timestamp para snapshot
            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            // Snapshot test para criação de usuário
            Snapshot.Match(responseObj, "CreateUser_ValidData");
        }

        [Fact]
        public async Task DeleteUser_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            await Factory.InitializeDatabaseAsync();
            var adminToken = GenerateJwtToken(1, "admin", Roles.Admin);
            Client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            // Act
            var response = await Client.DeleteAsync("/api/user/3");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            content.Should().BeEmpty();
            
            // Para NoContent, verificamos apenas o status code
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteUser_WithInvalidId_ShouldMatchSnapshot()
        {
            // Arrange
            await Factory.InitializeDatabaseAsync();
            var adminToken = GenerateJwtToken(1, "admin", Roles.Admin);
            Client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            // Act
            var response = await Client.DeleteAsync("/api/user/999");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().NotBeNullOrEmpty();
            
            // Snapshot test para erro de usuário não encontrado
            Snapshot.Match(content, "DeleteUser_InvalidId");
        }

        [Fact]
        public async Task ContractTests_ShouldValidateJsonStructure()
        {
            // Arrange
            await Factory.InitializeDatabaseAsync();
            var adminToken = GenerateJwtToken(1, "admin", Roles.Admin);
            Client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            // Act
            var response = await Client.GetAsync("/api/user/GetAll");
            var content = await response.Content.ReadAsStringAsync();

            // Assert - Validações de estrutura JSON além do snapshot
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            
            var users = JsonSerializer.Deserialize<JsonElement[]>(content);
            users.Should().NotBeNull();
            users!.Should().HaveCountGreaterThan(0);
            
            // Validar que cada usuário tem as propriedades esperadas
            foreach (var user in users!)
            {
                user.TryGetProperty("id", out _).Should().BeTrue();
                user.TryGetProperty("username", out _).Should().BeTrue();
                user.TryGetProperty("email", out _).Should().BeTrue();
                user.TryGetProperty("name", out _).Should().BeTrue();
                user.TryGetProperty("role", out _).Should().BeTrue();
            }
        }

        /// <summary>
        /// Gera um token JWT para testes de autenticação
        /// </summary>
        private string GenerateJwtToken(int userId, string username, Roles role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("test-secret-key-for-integration-tests-must-be-long-enough-to-pass-validation");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role.ToString()),
                    new Claim("SecurityStamp", Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
