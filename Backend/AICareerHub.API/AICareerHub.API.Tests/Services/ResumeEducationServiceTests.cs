using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;
using AICareerHub.API.Services;
using Moq;
using Xunit;

namespace AICareerHub.API.Tests.Services
{
    public class ResumeEducationServiceTests
    {
        private readonly Mock<IResumeRepository> _resumeRepositoryMock;
        private readonly Mock<IResumeEducationRepository> _educationRepositoryMock;
        private readonly ResumeEducationService _service;

        public ResumeEducationServiceTests()
        {
            _resumeRepositoryMock =
                new Mock<IResumeRepository>();

            _educationRepositoryMock =
                new Mock<IResumeEducationRepository>();

            _service =
                new ResumeEducationService(
                    _resumeRepositoryMock.Object,
                    _educationRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenEndYearIsBeforeStartYear_ThrowsArgumentException()
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

            var request = new CreateResumeEducationDto
            {
                Institution = "University",
                Degree = "BE",
                FieldOfStudy = "Computer Science",
                StartYear = 2020,
                EndYear = 2019
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
                "End year cannot be earlier than start year.",
                exception.Message);

            _educationRepositoryMock.Verify(
                repository =>
                    repository.CreateAsync(
                        It.IsAny<ResumeEducation>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenResumeDoesNotBelongToUser_ReturnsNull()
        {
            // Arrange
            var resumeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var request = new CreateResumeEducationDto
            {
                Institution = "University",
                Degree = "BE",
                FieldOfStudy = "Computer Science",
                StartYear = 2020,
                EndYear = 2024
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

            _educationRepositoryMock.Verify(
                repository =>
                    repository.CreateAsync(
                        It.IsAny<ResumeEducation>()),
                Times.Never);
        }
    }
}