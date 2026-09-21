using AICareerHub.API.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace AICareerHub.API.Tests.Integration
{
    public class JobApplicationApiTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public JobApplicationApiTests(
            CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateJobApplication_WithValidToken_ReturnsCreatedJob()
        {
            // Arrange
            using var client =
                _factory.CreateClient();

            await AuthenticateAsync(
                client,
                $"job-create-{Guid.NewGuid()}@example.com");

            var request = new CreateJobApplicationDto
            {
                CompanyName = "Microsoft",
                JobTitle = "Software Engineer",
                JobUrl = "https://example.com/job",
                Location = "Bangalore",
                Status = "Applied",
                AppliedDate = new DateOnly(2026, 9, 21),
                Notes = "Integration test application"
            };

            // Act
            var response =
                await client.PostAsJsonAsync(
                    "/api/job-applications",
                    request);

            // Assert
            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            var job =
                await response.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            Assert.NotNull(job);
            Assert.NotEqual(Guid.Empty, job.Id);

            Assert.Equal(
                "Microsoft",
                job.CompanyName);

            Assert.Equal(
                "Software Engineer",
                job.JobTitle);

            Assert.Equal(
                "Bangalore",
                job.Location);

            Assert.Equal(
                "Applied",
                job.Status);

            Assert.Equal(
                new DateOnly(2026, 9, 21),
                job.AppliedDate);
        }

        [Fact]
        public async Task GetJobApplication_WhenBelongsToAnotherUser_ReturnsNotFound()
        {
            // Arrange
            using var ownerClient =
                _factory.CreateClient();

            await AuthenticateAsync(
                ownerClient,
                $"job-owner-{Guid.NewGuid()}@example.com");

            var job =
                await CreateJobApplicationAsync(
                    ownerClient,
                    "Applied");

            using var otherUserClient =
                _factory.CreateClient();

            await AuthenticateAsync(
                otherUserClient,
                $"job-other-{Guid.NewGuid()}@example.com");

            // Act
            var response =
                await otherUserClient.GetAsync(
                    $"/api/job-applications/{job.Id}");

            // Assert
            Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
        }

        [Fact]
        public async Task GetStats_ReturnsStatisticsForCurrentUser()
        {
            // Arrange
            using var client =
                _factory.CreateClient();

            await AuthenticateAsync(
                client,
                $"job-stats-{Guid.NewGuid()}@example.com");

            await CreateJobApplicationAsync(
                client,
                "Applied");

            await CreateJobApplicationAsync(
                client,
                "Interview");

            await CreateJobApplicationAsync(
                client,
                "Offer");

            // Act
            var response =
                await client.GetAsync(
                    "/api/job-applications/stats");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var stats =
                await response.Content
                    .ReadFromJsonAsync<JobApplicationStatsDto>();

            Assert.NotNull(stats);

            Assert.Equal(3, stats.Total);
            Assert.Equal(1, stats.Applied);
            Assert.Equal(1, stats.Interview);
            Assert.Equal(1, stats.Offer);
            Assert.Equal(0, stats.Rejected);
            Assert.Equal(0, stats.Withdrawn);
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

        private static async Task<JobApplicationDto>
            CreateJobApplicationAsync(
                HttpClient client,
                string status)
        {
            var request = new CreateJobApplicationDto
            {
                CompanyName = "Integration Company",
                JobTitle = "Software Engineer",
                JobUrl = "https://example.com/job",
                Location = "Bangalore",
                Status = status,
                AppliedDate = new DateOnly(2026, 9, 21),
                Notes = "Created by integration test"
            };

            var response =
                await client.PostAsJsonAsync(
                    "/api/job-applications",
                    request);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            var job =
                await response.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            Assert.NotNull(job);

            return job;
        }
    }
}