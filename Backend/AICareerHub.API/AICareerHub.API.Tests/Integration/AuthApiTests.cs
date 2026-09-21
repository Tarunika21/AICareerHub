using AICareerHub.API.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AICareerHub.API.Tests.Integration
{
    public class AuthApiTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthApiTests(
            CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsCreatedUser()
        {
            // Arrange
            var request = new RegisterDto
            {
                FirstName = "Integration",
                LastName = "Test",
                Email =
                    $"integration-{Guid.NewGuid()}@example.com",
                Password = "Password123"
            };

            // Act
            var response =
                await _client.PostAsJsonAsync(
                    "/api/auth/register",
                    request);

            var responseBody =
                await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Expected 201 Created but received " +
                $"{(int)response.StatusCode} " +
                $"{response.StatusCode}. " +
                $"Response: {responseBody}"); ;

            var result =
                await response.Content
                    .ReadFromJsonAsync<UserDto>();

            Assert.NotNull(result);

            Assert.Equal(
                "Integration",
                result.FirstName);

            Assert.Equal(
                "Test",
                result.LastName);

            Assert.Equal(
                request.Email,
                result.Email);

            Assert.NotEqual(
                Guid.Empty,
                result.Id);
        }
    }
}