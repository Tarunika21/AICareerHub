using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;
using AICareerHub.API.Services;
using Moq;
using Xunit;

namespace AICareerHub.API.Tests.Services
{
    public class ResumeServiceTests
    {
        private readonly Mock<IResumeRepository> _resumeRepositoryMock;
        private readonly ResumeService _resumeService;

        public ResumeServiceTests()
        {
            _resumeRepositoryMock =
                new Mock<IResumeRepository>();

            _resumeService =
                new ResumeService(
                    _resumeRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOnlyUserResumes()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var resumes = new List<Resume>
            {
                new Resume
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = "Software Engineer Resume",
                    ProfessionalSummary = "Summary 1",
                    Skills = "Angular, ASP.NET Core",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Resume
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = "Full Stack Resume",
                    ProfessionalSummary = "Summary 2",
                    Skills = "React, .NET",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.GetByUserIdAsync(userId))
                .ReturnsAsync(resumes);

            // Act
            var result =
                (await _resumeService.GetAllAsync(userId))
                .ToList();

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Equal(
                "Software Engineer Resume",
                result[0].Title);

            Assert.Equal(
                "Full Stack Resume",
                result[1].Title);

            _resumeRepositoryMock.Verify(
                repository =>
                    repository.GetByUserIdAsync(userId),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenResumeBelongsToUser_ReturnsResume()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var resumeId = Guid.NewGuid();

            var resume = new Resume
            {
                Id = resumeId,
                UserId = userId,
                Title = "Software Engineer Resume",
                ProfessionalSummary = "Professional summary",
                Skills = "Angular, ASP.NET Core",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        resumeId,
                        userId))
                .ReturnsAsync(resume);

            // Act
            var result =
                await _resumeService.GetByIdAsync(
                    resumeId,
                    userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(resumeId, result.Id);
            Assert.Equal(
                "Software Engineer Resume",
                result.Title);

            Assert.Equal(
                "Professional summary",
                result.ProfessionalSummary);

            Assert.Equal(
                "Angular, ASP.NET Core",
                result.Skills);

            _resumeRepositoryMock.Verify(
                repository =>
                    repository.GetByIdAndUserIdAsync(
                        resumeId,
                        userId),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_CreatesNormalizedResume()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new CreateResumeDto
            {
                Title = "  Software Engineer Resume  ",
                ProfessionalSummary =
                    "  Full-stack software engineer  ",
                Skills =
                    "  Angular, ASP.NET Core, PostgreSQL  "
            };

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.CreateAsync(
                        It.IsAny<Resume>()))
                .ReturnsAsync(
                    (Resume resume) => resume);

            // Act
            var result =
                await _resumeService.CreateAsync(
                    userId,
                    request);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);

            Assert.Equal(
                "Software Engineer Resume",
                result.Title);

            Assert.Equal(
                "Full-stack software engineer",
                result.ProfessionalSummary);

            Assert.Equal(
                "Angular, ASP.NET Core, PostgreSQL",
                result.Skills);

            _resumeRepositoryMock.Verify(
                repository =>
                    repository.CreateAsync(
                        It.Is<Resume>(resume =>
                            resume.UserId == userId &&
                            resume.Title ==
                                "Software Engineer Resume" &&
                            resume.ProfessionalSummary ==
                                "Full-stack software engineer" &&
                            resume.Skills ==
                                "Angular, ASP.NET Core, PostgreSQL")),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenResumeDoesNotBelongToUser_ReturnsNull()
        {
            // Arrange
            var resumeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        resumeId,
                        userId))
                .ReturnsAsync((Resume?)null);

            // Act
            var result =
                await _resumeService.GetByIdAsync(
                    resumeId,
                    userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_WhenResumeBelongsToUser_UpdatesAndReturnsResume()
        {
            // Arrange
            var resumeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var resume = new Resume
            {
                Id = resumeId,
                UserId = userId,
                Title = "Old Resume",
                ProfessionalSummary = "Old summary",
                Skills = "Angular",
                CreatedAt = DateTime.UtcNow.AddMonths(-1),
                UpdatedAt = DateTime.UtcNow.AddMonths(-1)
            };

            var originalUpdatedAt = resume.UpdatedAt;

            var request = new UpdateResumeDto
            {
                Title = "  Updated Resume  ",
                ProfessionalSummary = "  Updated summary  ",
                Skills = "  Angular, ASP.NET Core  "
            };

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        resumeId,
                        userId))
                .ReturnsAsync(resume);

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.UpdateAsync(
                        It.IsAny<Resume>()))
                .ReturnsAsync(
                    (Resume updatedResume) => updatedResume);

            // Act
            var result =
                await _resumeService.UpdateAsync(
                    resumeId,
                    userId,
                    request);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Updated Resume",
                result.Title);

            Assert.Equal(
                "Updated summary",
                result.ProfessionalSummary);

            Assert.Equal(
                "Angular, ASP.NET Core",
                result.Skills);

            Assert.True(
                result.UpdatedAt > originalUpdatedAt);

            _resumeRepositoryMock.Verify(
                repository =>
                    repository.UpdateAsync(
                        It.Is<Resume>(updatedResume =>
                            updatedResume.Id == resumeId &&
                            updatedResume.UserId == userId &&
                            updatedResume.Title ==
                                "Updated Resume" &&
                            updatedResume.ProfessionalSummary ==
                                "Updated summary" &&
                            updatedResume.Skills ==
                                "Angular, ASP.NET Core")),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenResumeDoesNotBelongToUser_ReturnsNull()
        {
            // Arrange
            var resumeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var request = new UpdateResumeDto
            {
                Title = "Updated Resume",
                ProfessionalSummary = "Updated summary",
                Skills = "Angular, ASP.NET Core"
            };

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        resumeId,
                        userId))
                .ReturnsAsync((Resume?)null);

            // Act
            var result =
                await _resumeService.UpdateAsync(
                    resumeId,
                    userId,
                    request);

            // Assert
            Assert.Null(result);

            _resumeRepositoryMock.Verify(
                repository =>
                    repository.UpdateAsync(
                        It.IsAny<Resume>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenResumeBelongsToUser_DeletesAndReturnsTrue()
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

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        resumeId,
                        userId))
                .ReturnsAsync(resume);

            // Act
            var result =
                await _resumeService.DeleteAsync(
                    resumeId,
                    userId);

            // Assert
            Assert.True(result);

            _resumeRepositoryMock.Verify(
                repository =>
                    repository.DeleteAsync(resume),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenResumeDoesNotBelongToUser_ReturnsFalse()
        {
            // Arrange
            var resumeId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _resumeRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAndUserIdAsync(
                        resumeId,
                        userId))
                .ReturnsAsync((Resume?)null);

            // Act
            var result =
                await _resumeService.DeleteAsync(
                    resumeId,
                    userId);

            // Assert
            Assert.False(result);

            _resumeRepositoryMock.Verify(
                repository =>
                    repository.DeleteAsync(
                        It.IsAny<Resume>()),
                Times.Never);
        }
    }
}