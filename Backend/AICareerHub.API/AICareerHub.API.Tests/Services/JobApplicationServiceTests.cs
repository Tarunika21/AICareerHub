using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;
using AICareerHub.API.Services;
using Moq;
using Xunit;

namespace AICareerHub.API.Tests.Services
{
    public class JobApplicationServiceTests
    {
        private readonly Mock<IJobApplicationRepository> _repositoryMock;
        private readonly JobApplicationService _service;

        public JobApplicationServiceTests()
        {
            _repositoryMock =
                new Mock<IJobApplicationRepository>();

            _service =
                new JobApplicationService(
                    _repositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesNormalizedJobApplication()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new CreateJobApplicationDto
            {
                CompanyName = "  Microsoft  ",
                JobTitle = "  Software Engineer  ",
                JobUrl = "  https://example.com/job  ",
                Location = "  Bangalore  ",
                Status = "  interview  ",
                AppliedDate = new DateOnly(2026, 9, 20),
                Notes = "  Technical interview scheduled  "
            };

            _repositoryMock
                .Setup(repository =>
                    repository.CreateAsync(
                        It.IsAny<JobApplication>()))
                .ReturnsAsync(
                    (JobApplication job) => job);

            // Act
            var result =
                await _service.CreateAsync(
                    userId,
                    request);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);

            Assert.Equal(
                "Microsoft",
                result.CompanyName);

            Assert.Equal(
                "Software Engineer",
                result.JobTitle);

            Assert.Equal(
                "https://example.com/job",
                result.JobUrl);

            Assert.Equal(
                "Bangalore",
                result.Location);

            Assert.Equal(
                "Interview",
                result.Status);

            Assert.Equal(
                "Technical interview scheduled",
                result.Notes);

            _repositoryMock.Verify(
                repository =>
                    repository.CreateAsync(
                        It.Is<JobApplication>(job =>
                            job.UserId == userId &&
                            job.CompanyName == "Microsoft" &&
                            job.JobTitle ==
                                "Software Engineer" &&
                            job.Location == "Bangalore" &&
                            job.Status == "Interview" &&
                            job.Notes ==
                                "Technical interview scheduled")),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidStatus_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new CreateJobApplicationDto
            {
                CompanyName = "Microsoft",
                JobTitle = "Software Engineer",
                Location = "Bangalore",
                Status = "Pending",
                AppliedDate = new DateOnly(2026, 9, 20)
            };

            // Act
            var exception =
                await Assert.ThrowsAsync<ArgumentException>(
                    () => _service.CreateAsync(
                        userId,
                        request));

            // Assert
            Assert.Equal(
                "Status must be Applied, Interview, Offer, Rejected, or Withdrawn.",
                exception.Message);

            _repositoryMock.Verify(
                repository =>
                    repository.CreateAsync(
                        It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Theory]
        [InlineData("applied", "Applied")]
        [InlineData("INTERVIEW", "Interview")]
        [InlineData(" offer ", "Offer")]
        [InlineData("Rejected", "Rejected")]
        [InlineData("withdrawn", "Withdrawn")]
        public async Task CreateAsync_WithValidStatus_NormalizesStatus(
            string inputStatus,
            string expectedStatus)
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new CreateJobApplicationDto
            {
                CompanyName = "Company",
                JobTitle = "Software Engineer",
                Location = "Remote",
                Status = inputStatus,
                AppliedDate = new DateOnly(2026, 9, 20)
            };

            _repositoryMock
                .Setup(repository =>
                    repository.CreateAsync(
                        It.IsAny<JobApplication>()))
                .ReturnsAsync(
                    (JobApplication job) => job);

            // Act
            var result =
                await _service.CreateAsync(
                    userId,
                    request);

            // Assert
            Assert.Equal(
                expectedStatus,
                result.Status);
        }

        [Fact]
        public async Task UpdateAsync_WhenJobExists_UpdatesAndReturnsJob()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var job = new JobApplication
            {
                Id = jobId,
                UserId = userId,
                CompanyName = "Old Company",
                JobTitle = "Developer",
                Location = "Coimbatore",
                Status = "Applied",
                AppliedDate = new DateOnly(2026, 9, 1),
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            };

            var originalUpdatedAt = job.UpdatedAt;

            var request = new UpdateJobApplicationDto
            {
                CompanyName = "  Microsoft  ",
                JobTitle = "  Software Engineer  ",
                JobUrl = "  https://example.com/job  ",
                Location = "  Bangalore  ",
                Status = "  INTERVIEW  ",
                AppliedDate = new DateOnly(2026, 9, 1),
                Notes = "  Interview scheduled  "
            };

            _repositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        jobId,
                        userId))
                .ReturnsAsync(job);

            _repositoryMock
                .Setup(repository =>
                    repository.UpdateAsync(
                        It.IsAny<JobApplication>()))
                .ReturnsAsync(
                    (JobApplication updatedJob) => updatedJob);

            // Act
            var result =
                await _service.UpdateAsync(
                    jobId,
                    userId,
                    request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Microsoft", result.CompanyName);
            Assert.Equal("Software Engineer", result.JobTitle);
            Assert.Equal("Bangalore", result.Location);
            Assert.Equal("Interview", result.Status);
            Assert.Equal("Interview scheduled", result.Notes);
            Assert.True(result.UpdatedAt > originalUpdatedAt);

            _repositoryMock.Verify(
                repository =>
                    repository.UpdateAsync(
                        It.Is<JobApplication>(updatedJob =>
                            updatedJob.Id == jobId &&
                            updatedJob.UserId == userId &&
                            updatedJob.Status == "Interview")),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenJobDoesNotBelongToUser_ReturnsNull()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var request = new UpdateJobApplicationDto
            {
                CompanyName = "Company",
                JobTitle = "Software Engineer",
                Location = "Remote",
                Status = "Applied",
                AppliedDate = new DateOnly(2026, 9, 20)
            };

            _repositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        jobId,
                        userId))
                .ReturnsAsync((JobApplication?)null);

            // Act
            var result =
                await _service.UpdateAsync(
                    jobId,
                    userId,
                    request);

            // Assert
            Assert.Null(result);

            _repositoryMock.Verify(
                repository =>
                    repository.UpdateAsync(
                        It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidStatus_ThrowsArgumentException()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var request = new UpdateJobApplicationDto
            {
                CompanyName = "Company",
                JobTitle = "Software Engineer",
                Location = "Remote",
                Status = "Pending",
                AppliedDate = new DateOnly(2026, 9, 20)
            };

            // Act
            var exception =
                await Assert.ThrowsAsync<ArgumentException>(
                    () => _service.UpdateAsync(
                        jobId,
                        userId,
                        request));

            // Assert
            Assert.Equal(
                "Status must be Applied, Interview, Offer, Rejected, or Withdrawn.",
                exception.Message);

            _repositoryMock.Verify(
                repository =>
                    repository.UpdateAsync(
                        It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenJobExists_DeletesAndReturnsTrue()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var job = new JobApplication
            {
                Id = jobId,
                UserId = userId,
                CompanyName = "Company",
                JobTitle = "Software Engineer",
                Location = "Remote",
                Status = "Applied",
                AppliedDate = new DateOnly(2026, 9, 20),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        jobId,
                        userId))
                .ReturnsAsync(job);

            // Act
            var result =
                await _service.DeleteAsync(
                    jobId,
                    userId);

            // Assert
            Assert.True(result);

            _repositoryMock.Verify(
                repository =>
                    repository.DeleteAsync(job),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenJobDoesNotBelongToUser_ReturnsFalse()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _repositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        jobId,
                        userId))
                .ReturnsAsync((JobApplication?)null);

            // Act
            var result =
                await _service.DeleteAsync(
                    jobId,
                    userId);

            // Assert
            Assert.False(result);

            _repositoryMock.Verify(
                repository =>
                    repository.DeleteAsync(
                        It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task SearchAsync_WithInvalidStatus_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var exception =
                await Assert.ThrowsAsync<ArgumentException>(
                    () => _service.SearchAsync(
                        userId,
                        "Pending",
                        null));

            // Assert
            Assert.Equal(
                "Status must be Applied, Interview, Offer, Rejected, or Withdrawn.",
                exception.Message);

            _repositoryMock.Verify(
                repository =>
                    repository.SearchAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<string?>(),
                        It.IsAny<string?>()),
                Times.Never);
        }

        [Fact]
        public async Task SearchAsync_WithValidFilters_ReturnsMatchingJobs()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var jobs = new List<JobApplication>
    {
        new JobApplication
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyName = "Microsoft",
            JobTitle = "Software Engineer",
            Location = "Bangalore",
            Status = "Interview",
            AppliedDate = new DateOnly(2026, 9, 20),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }
    };

            _repositoryMock
                .Setup(repository =>
                    repository.SearchAsync(
                        userId,
                        "Interview",
                        "Microsoft"))
                .ReturnsAsync(jobs);

            // Act
            var result =
                (await _service.SearchAsync(
                    userId,
                    "Interview",
                    "Microsoft"))
                .ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Microsoft", result[0].CompanyName);
            Assert.Equal("Interview", result[0].Status);

            _repositoryMock.Verify(
                repository =>
                    repository.SearchAsync(
                        userId,
                        "Interview",
                        "Microsoft"),
                Times.Once);
        }

        [Fact]
        public async Task GetStatsAsync_ReturnsRepositoryStatistics()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var stats = new JobApplicationStatsDto
            {
                Total = 10,
                Applied = 3,
                Interview = 2,
                Offer = 1,
                Rejected = 3,
                Withdrawn = 1
            };

            _repositoryMock
                .Setup(repository =>
                    repository.GetStatsAsync(userId))
                .ReturnsAsync(stats);

            // Act
            var result =
                await _service.GetStatsAsync(userId);

            // Assert
            Assert.Equal(10, result.Total);
            Assert.Equal(3, result.Applied);
            Assert.Equal(2, result.Interview);
            Assert.Equal(1, result.Offer);
            Assert.Equal(3, result.Rejected);
            Assert.Equal(1, result.Withdrawn);

            _repositoryMock.Verify(
                repository =>
                    repository.GetStatsAsync(userId),
                Times.Once);
        }
    }
}