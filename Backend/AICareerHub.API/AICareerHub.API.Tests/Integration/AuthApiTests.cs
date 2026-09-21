using AICareerHub.API.DTOs;
using System.Net;
using System.Net.Http.Headers;
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
                $"Response: {responseBody}");

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

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var email =
                $"login-{Guid.NewGuid()}@example.com";

            var password = "Password123";

            await RegisterUserAsync(
                email,
                password);

            var loginRequest = new LoginDto
            {
                Email = email,
                Password = password
            };

            // Act
            var response =
                await _client.PostAsJsonAsync(
                    "/api/auth/login",
                    loginRequest);

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var result =
                await response.Content
                    .ReadFromJsonAsync<AuthResponseDto>();

            Assert.NotNull(result);

            Assert.False(
                string.IsNullOrWhiteSpace(result.Token));

            Assert.Equal(
                email,
                result.User.Email);

            Assert.True(
                result.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var email =
                $"invalid-login-{Guid.NewGuid()}@example.com";

            await RegisterUserAsync(
                email,
                "Password123");

            var loginRequest = new LoginDto
            {
                Email = email,
                Password = "WrongPassword123"
            };

            // Act
            var response =
                await _client.PostAsJsonAsync(
                    "/api/auth/login",
                    loginRequest);

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task GetCurrentUser_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response =
                await _client.GetAsync(
                    "/api/users/me");

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task GetCurrentUser_WithValidToken_ReturnsCurrentUser()
        {
            // Arrange
            var email =
                $"protected-{Guid.NewGuid()}@example.com";

            var password = "Password123";

            await RegisterUserAsync(
                email,
                password);

            var loginRequest = new LoginDto
            {
                Email = email,
                Password = password
            };

            var loginResponse =
                await _client.PostAsJsonAsync(
                    "/api/auth/login",
                    loginRequest);

            Assert.Equal(
                HttpStatusCode.OK,
                loginResponse.StatusCode);

            var loginResult =
                await loginResponse.Content
                    .ReadFromJsonAsync<AuthResponseDto>();

            Assert.NotNull(loginResult);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResult.Token);

            // Act
            var response =
                await _client.GetAsync(
                    "/api/users/me");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var user =
                await response.Content
                    .ReadFromJsonAsync<UserDto>();

            Assert.NotNull(user);

            Assert.Equal(
                email,
                user.Email);

            Assert.Equal(
                "Integration",
                user.FirstName);

            Assert.Equal(
                "Test",
                user.LastName);

            Assert.Equal(
                loginResult.User.Id,
                user.Id);
        }

        private async Task<UserDto> RegisterUserAsync(
            string email,
            string password)
        {
            var registerRequest = new RegisterDto
            {
                FirstName = "Integration",
                LastName = "Test",
                Email = email,
                Password = password
            };

            var response =
                await _client.PostAsJsonAsync(
                    "/api/auth/register",
                    registerRequest);

            var responseBody =
                await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Registration failed. Expected 201 Created " +
                $"but received {(int)response.StatusCode} " +
                $"{response.StatusCode}. " +
                $"Response: {responseBody}");

            var user =
                await response.Content
                    .ReadFromJsonAsync<UserDto>();

            Assert.NotNull(user);

            return user;
        }
    }
}