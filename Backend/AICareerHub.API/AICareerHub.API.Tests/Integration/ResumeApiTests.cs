using AICareerHub.API.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace AICareerHub.API.Tests.Integration
{
    public class ResumeApiTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public ResumeApiTests(
            CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateResume_WithValidToken_ReturnsCreatedResume()
        {
            // Arrange
            using var client =
                _factory.CreateClient();

            await AuthenticateAsync(
                client,
                $"resume-create-{Guid.NewGuid()}@example.com");

            var request = new CreateResumeDto
            {
                Title = "  Software Engineer Resume  ",
                ProfessionalSummary =
                    "  Full-stack software engineer  ",
                Skills =
                    "  Angular, ASP.NET Core, PostgreSQL  "
            };

            // Act
            var response =
                await client.PostAsJsonAsync(
                    "/api/resumes",
                    request);

            // Assert
            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            var resume =
                await response.Content
                    .ReadFromJsonAsync<ResumeDto>();

            Assert.NotNull(resume);
            Assert.NotEqual(Guid.Empty, resume.Id);

            Assert.Equal(
                "Software Engineer Resume",
                resume.Title);

            Assert.Equal(
                "Full-stack software engineer",
                resume.ProfessionalSummary);

            Assert.Equal(
                "Angular, ASP.NET Core, PostgreSQL",
                resume.Skills);
        }

        [Fact]
        public async Task GetResume_WhenUserOwnsResume_ReturnsResume()
        {
            // Arrange
            using var client =
                _factory.CreateClient();

            await AuthenticateAsync(
                client,
                $"resume-owner-{Guid.NewGuid()}@example.com");

            var resume =
                await CreateResumeAsync(client);

            // Act
            var response =
                await client.GetAsync(
                    $"/api/resumes/{resume.Id}");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var result =
                await response.Content
                    .ReadFromJsonAsync<ResumeDto>();

            Assert.NotNull(result);
            Assert.Equal(resume.Id, result.Id);
            Assert.Equal(resume.Title, result.Title);
            Assert.Equal(resume.Skills, result.Skills);
        }

        [Fact]
        public async Task GetResume_WhenResumeBelongsToAnotherUser_ReturnsNotFound()
        {
            // Arrange
            using var ownerClient =
                _factory.CreateClient();

            await AuthenticateAsync(
                ownerClient,
                $"owner-{Guid.NewGuid()}@example.com");

            var resume =
                await CreateResumeAsync(ownerClient);

            using var otherUserClient =
                _factory.CreateClient();

            await AuthenticateAsync(
                otherUserClient,
                $"other-{Guid.NewGuid()}@example.com");

            // Act
            var response =
                await otherUserClient.GetAsync(
                    $"/api/resumes/{resume.Id}");

            // Assert
            Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
        }

        private static async Task AuthenticateAsync(
            HttpClient client,
            string email)
        {
            const string password = "Password123";

            var registerRequest = new RegisterDto
            {
                FirstName = "Integration",
                LastName = "Test",
                Email = email,
                Password = password
            };

            var registerResponse =
                await client.PostAsJsonAsync(
                    "/api/auth/register",
                    registerRequest);

            Assert.Equal(
                HttpStatusCode.Created,
                registerResponse.StatusCode);

            var loginRequest = new LoginDto
            {
                Email = email,
                Password = password
            };

            var loginResponse =
                await client.PostAsJsonAsync(
                    "/api/auth/login",
                    loginRequest);

            Assert.Equal(
                HttpStatusCode.OK,
                loginResponse.StatusCode);

            var loginResult =
                await loginResponse.Content
                    .ReadFromJsonAsync<AuthResponseDto>();

            Assert.NotNull(loginResult);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResult.Token);
        }

        private static async Task<ResumeDto> CreateResumeAsync(
            HttpClient client)
        {
            var request = new CreateResumeDto
            {
                Title = "Integration Test Resume",
                ProfessionalSummary =
                    "Integration test summary",
                Skills =
                    "Angular, ASP.NET Core"
            };

            var response =
                await client.PostAsJsonAsync(
                    "/api/resumes",
                    request);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            var resume =
                await response.Content
                    .ReadFromJsonAsync<ResumeDto>();

            Assert.NotNull(resume);

            return resume;
        }
    }
}