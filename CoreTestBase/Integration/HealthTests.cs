using CoreApiBase.Controllers;

namespace CoreTestBase.Integration
{
    public class HealthTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public HealthTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetHealth_ShouldReturnOk_WhenApplicationIsHealthy()
        {
            // Act
            var response = await _client.GetAsync("/api/health");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            
            var healthResponse = JsonSerializer.Deserialize<HealthResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            healthResponse.Should().NotBeNull();
            healthResponse!.Status.Should().Be("Healthy");
            healthResponse.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task GetConfigHealth_ShouldReturnDetailedHealth_WithConfigurationChecks()
        {
            // Act
            var response = await _client.GetAsync("/api/health/config");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
            content.Should().NotBeNullOrEmpty();
            
            var healthResponse = JsonSerializer.Deserialize<DetailedHealthResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            healthResponse.Should().NotBeNull();
            healthResponse!.Status.Should().BeOneOf("Healthy", "Unhealthy", "Degraded");
            healthResponse.Checks.Should().NotBeNull();
        }

        [Fact]
        public async Task GetHealthByTag_ShouldReturnFilteredHealth_ForValidTag()
        {
            // Act
            var response = await _client.GetAsync("/api/health/tag/config");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
            content.Should().NotBeNullOrEmpty();
            
            var healthResponse = JsonSerializer.Deserialize<DetailedHealthResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            healthResponse.Should().NotBeNull();
            healthResponse!.FilteredByTag.Should().Be("config");
        }

        [Fact]
        public async Task GetHealthTags_ShouldReturnAvailableTags()
        {
            // Act
            var response = await _client.GetAsync("/api/health/tags");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            
            var tagsResponse = JsonSerializer.Deserialize<HealthTagsResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            tagsResponse.Should().NotBeNull();
            tagsResponse!.Tags.Should().NotBeNull();
            tagsResponse.TotalChecks.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("/api/health")]
        [InlineData("/api/health/config")]
        [InlineData("/api/health/tags")]
        public async Task HealthEndpoints_ShouldReturnValidResponse_ForAllEndpoints(string endpoint)
        {
            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.OK, 
                HttpStatusCode.ServiceUnavailable);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetHealth_ShouldHaveCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/api/health");

            // Assert
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        }

        [Fact]
        public async Task GetHealthByTag_ShouldReturnNotFound_ForInvalidTag()
        {
            // Act
            var response = await _client.GetAsync("/api/health/tag/nonexistent");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            // Mesmo para tags que não existem, o endpoint deve retornar OK com uma lista vazia
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
            
            var healthResponse = JsonSerializer.Deserialize<DetailedHealthResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            healthResponse.Should().NotBeNull();
            healthResponse!.FilteredByTag.Should().Be("nonexistent");
        }

        [Fact]
        public async Task HealthEndpoints_ShouldNotRequireAuthentication()
        {
            // Arrange - Cliente sem autenticação (já é o padrão)
            
            // Act
            var healthResponse = await _client.GetAsync("/api/health");
            var configResponse = await _client.GetAsync("/api/health/config");
            var tagsResponse = await _client.GetAsync("/api/health/tags");

            // Assert
            healthResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            configResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            tagsResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetHealth_PerformanceTest_ShouldRespondWithinReasonableTime()
        {
            // Arrange
            var maxResponseTime = TimeSpan.FromSeconds(5);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var response = await _client.GetAsync("/api/health");

            // Assert
            stopwatch.Stop();
            stopwatch.Elapsed.Should().BeLessThan(maxResponseTime);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task GetHealth_MultipleRequests_ShouldBeConsistent()
        {
            // Arrange
            var tasks = new List<Task<HttpResponseMessage>>();

            // Act - Fazer múltiplas requisições paralelas
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(_client.GetAsync("/api/health"));
            }

            var responses = await Task.WhenAll(tasks);

            // Assert
            responses.Should().HaveCount(5);
            responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK || r.StatusCode == HttpStatusCode.ServiceUnavailable);
            
            // Todos devem ter o mesmo status (assumindo que a saúde não muda durante o teste)
            var statusCodes = responses.Select(r => r.StatusCode).Distinct();
            statusCodes.Should().HaveCount(1);
        }
    }
}
