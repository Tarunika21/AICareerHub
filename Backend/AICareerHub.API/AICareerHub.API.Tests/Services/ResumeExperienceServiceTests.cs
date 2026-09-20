using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;
using AICareerHub.API.Services;
using Moq;
using Xunit;

namespace AICareerHub.API.Tests.Services
{
    public class ResumeExperienceServiceTests
    {
        private readonly Mock<IResumeRepository> _resumeRepositoryMock;
        private readonly Mock<IResumeExperienceRepository> _experienceRepositoryMock;
        private readonly ResumeExperienceService _service;

        public ResumeExperienceServiceTests()
        {
            _resumeRepositoryMock =
                new Mock<IResumeRepository>();

            _experienceRepositoryMock =
                new Mock<IResumeExperienceRepository>();

            _service =
                new ResumeExperienceService(
                    _resumeRepositoryMock.Object,
                    _experienceRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenEndDateIsBeforeStartDate_ThrowsArgumentException()
        {
            // Arrange
            var resumeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var resume = new Resume
            {
                Id = resumeId,
                UserId = userId,
                Title = "Resume",
                Skills = "Angular",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var request = new CreateResumeExperienceDto
            {
                Company = "Company",
                JobTitle = "Software Engineer",
                StartDate = new DateOnly(2026, 1, 1),
                EndDate = new DateOnly(2025, 12, 31),
                IsCurrent = false,
                Description = "Development work"
            };

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        resumeId,
                        userId))
                .ReturnsAsync(resume);

            // Act
            var exception =
                await Assert.ThrowsAsync<ArgumentException>(
                    () => _service.CreateAsync(
                        resumeId,
                        userId,
                        request));

            // Assert
            Assert.Equal(
                "End date cannot be earlier than start date.",
                exception.Message);

            _experienceRepositoryMock.Verify(
                repository =>
                    repository.CreateAsync(
                        It.IsAny<ResumeExperience>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenCurrentExperience_IgnoresEndDate()
        {
            // Arrange
            var resumeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var resume = new Resume
            {
                Id = resumeId,
                UserId = userId,
                Title = "Resume",
                Skills = "Angular",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var request = new CreateResumeExperienceDto
            {
                Company = "  Test Company  ",
                JobTitle = "  Software Engineer  ",
                StartDate = new DateOnly(2025, 1, 1),
                EndDate = new DateOnly(2026, 1, 1),
                IsCurrent = true,
                Description = "  Building applications  "
            };

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        resumeId,
                        userId))
                .ReturnsAsync(resume);

            _experienceRepositoryMock
                .Setup(repository =>
                    repository.CreateAsync(
                        It.IsAny<ResumeExperience>()))
                .ReturnsAsync(
                    (ResumeExperience experience) =>
                        experience);

            // Act
            var result =
                await _service.CreateAsync(
                    resumeId,
                    userId,
                    request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCurrent);
            Assert.Null(result.EndDate);

            Assert.Equal(
                "Test Company",
                result.Company);

            Assert.Equal(
                "Software Engineer",
                result.JobTitle);

            Assert.Equal(
                "Building applications",
                result.Description);
        }

        [Fact]
        public async Task CreateAsync_WhenResumeDoesNotBelongToUser_ReturnsNull()
        {
            // Arrange
            var resumeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var request = new CreateResumeExperienceDto
            {
                Company = "Company",
                JobTitle = "Software Engineer",
                StartDate = new DateOnly(2025, 1, 1),
                IsCurrent = true
            };

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        resumeId,
                        userId))
                .ReturnsAsync((Resume?)null);

            // Act
            var result =
                await _service.CreateAsync(
                    resumeId,
                    userId,
                    request);

            // Assert
            Assert.Null(result);

            _experienceRepositoryMock.Verify(
                repository =>
                    repository.CreateAsync(
                        It.IsAny<ResumeExperience>()),
                Times.Never);
        }
    }
}