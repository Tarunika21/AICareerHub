using AICareerHub.API.Common.Exceptions;
using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;
using AICareerHub.API.Services;
using Moq;

namespace AICareerHub.API.Tests.Services
{
    public class CareerProfileServiceTests
    {
        private readonly Mock<ICareerProfileRepository> _careerProfileRepositoryMock;
        private readonly CareerProfileService _careerProfileService;

        public CareerProfileServiceTests()
        {
            _careerProfileRepositoryMock =
                new Mock<ICareerProfileRepository>();

            _careerProfileService =
                new CareerProfileService(
                    _careerProfileRepositoryMock.Object);
        }

        [Fact]
        public async Task GetByUserIdAsync_WhenProfileExists_ReturnsProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var profileId = Guid.NewGuid();

            var profile = new CareerProfile
            {
                Id = profileId,
                UserId = userId,
                CurrentJobTitle = "Software Engineer",
                YearsOfExperience = 2.2m,
                Skills = "Angular, ASP.NET Core",
                CurrentLocation = "Coimbatore",
                PreferredLocations = "Remote, Bangalore",
                TargetRole = "Full Stack Software Engineer",
                TargetSalary = 2000000,
                ProfessionalSummary = "Full-stack developer",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _careerProfileRepositoryMock
                .Setup(repository =>
                    repository.GetByUserIdAsync(userId))
                .ReturnsAsync(profile);

            // Act
            var result =
                await _careerProfileService.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(profileId, result.Id);
            Assert.Equal("Software Engineer", result.CurrentJobTitle);
            Assert.Equal(2.2m, result.YearsOfExperience);
            Assert.Equal("Angular, ASP.NET Core", result.Skills);
            Assert.Equal("Coimbatore", result.CurrentLocation);
            Assert.Equal("Remote, Bangalore", result.PreferredLocations);
            Assert.Equal("Full Stack Software Engineer", result.TargetRole);
            Assert.Equal(2000000, result.TargetSalary);
            Assert.Equal("Full-stack developer", result.ProfessionalSummary);
        }

        [Fact]
        public async Task GetByUserIdAsync_WhenProfileDoesNotExist_ReturnsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _careerProfileRepositoryMock
                .Setup(repository =>
                    repository.GetByUserIdAsync(userId))
                .ReturnsAsync((CareerProfile?)null);

            // Act
            var result =
                await _careerProfileService.GetByUserIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_WhenProfileDoesNotExist_CreatesAndReturnsProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new CreateCareerProfileDto
            {
                CurrentJobTitle = "  Software Engineer  ",
                YearsOfExperience = 2.2m,
                Skills = "  Angular, ASP.NET Core  ",
                CurrentLocation = "  Coimbatore  ",
                PreferredLocations = "  Remote, Bangalore  ",
                TargetRole = "  Full Stack Software Engineer  ",
                TargetSalary = 2000000,
                ProfessionalSummary = "  Full-stack developer  "
            };

            _careerProfileRepositoryMock
                .Setup(repository =>
                    repository.GetByUserIdAsync(userId))
                .ReturnsAsync((CareerProfile?)null);

            _careerProfileRepositoryMock
                .Setup(repository =>
                    repository.CreateAsync(
                        It.IsAny<CareerProfile>()))
                .ReturnsAsync(
                    (CareerProfile profile) => profile);

            // Act
            var result =
                await _careerProfileService.CreateAsync(
                    userId,
                    request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(
                "Software Engineer",
                result.CurrentJobTitle);

            Assert.Equal(
                "Angular, ASP.NET Core",
                result.Skills);

            Assert.Equal(
                "Coimbatore",
                result.CurrentLocation);

            Assert.Equal(
                "Remote, Bangalore",
                result.PreferredLocations);

            Assert.Equal(
                "Full Stack Software Engineer",
                result.TargetRole);

            Assert.Equal(
                "Full-stack developer",
                result.ProfessionalSummary);

            Assert.Equal(
                2.2m,
                result.YearsOfExperience);

            Assert.Equal(
                2000000,
                result.TargetSalary);

            _careerProfileRepositoryMock.Verify(
                repository =>
                    repository.CreateAsync(
                        It.Is<CareerProfile>(profile =>
                            profile.UserId == userId &&
                            profile.CurrentJobTitle ==
                                "Software Engineer" &&
                            profile.Skills ==
                                "Angular, ASP.NET Core" &&
                            profile.CurrentLocation ==
                                "Coimbatore" &&
                            profile.PreferredLocations ==
                                "Remote, Bangalore" &&
                            profile.TargetRole ==
                                "Full Stack Software Engineer" &&
                            profile.ProfessionalSummary ==
                                "Full-stack developer")),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenProfileAlreadyExists_ThrowsConflictException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var existingProfile = new CareerProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CurrentJobTitle = "Software Engineer",
                Skills = "Angular",
                CurrentLocation = "Coimbatore",
                PreferredLocations = "Remote",
                TargetRole = "Full Stack Engineer",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var request = new CreateCareerProfileDto
            {
                CurrentJobTitle = "Senior Software Engineer",
                YearsOfExperience = 3,
                Skills = "Angular, ASP.NET Core",
                CurrentLocation = "Coimbatore",
                PreferredLocations = "Remote",
                TargetRole = "Senior Full Stack Engineer"
            };

            _careerProfileRepositoryMock
                .Setup(repository =>
                    repository.GetByUserIdAsync(userId))
                .ReturnsAsync(existingProfile);

            // Act
            var exception =
                await Assert.ThrowsAsync<ConflictException>(
                    () =>
                        _careerProfileService.CreateAsync(
                            userId,
                            request));

            // Assert
            Assert.Equal(
                "Career profile already exists.",
                exception.Message);

            _careerProfileRepositoryMock.Verify(
                repository =>
                    repository.CreateAsync(
                        It.IsAny<CareerProfile>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenProfileExists_UpdatesAndReturnsProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var existingProfile = new CareerProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CurrentJobTitle = "Software Engineer",
                YearsOfExperience = 2,
                Skills = "Angular",
                CurrentLocation = "Coimbatore",
                PreferredLocations = "Coimbatore",
                TargetRole = "Software Engineer",
                TargetSalary = 1500000,
                ProfessionalSummary = "Old summary",
                CreatedAt = DateTime.UtcNow.AddMonths(-1),
                UpdatedAt = DateTime.UtcNow.AddMonths(-1)
            };

            var originalUpdatedAt = existingProfile.UpdatedAt;

            var request = new UpdateCareerProfileDto
            {
                CurrentJobTitle = "  Senior Software Engineer  ",
                YearsOfExperience = 3,
                Skills = "  Angular, ASP.NET Core  ",
                CurrentLocation = "  Coimbatore  ",
                PreferredLocations = "  Remote, Bangalore  ",
                TargetRole = "  Full Stack Engineer  ",
                TargetSalary = 2000000,
                ProfessionalSummary = "  Updated summary  "
            };

            _careerProfileRepositoryMock
                .Setup(repository =>
                    repository.GetByUserIdAsync(userId))
                .ReturnsAsync(existingProfile);

            _careerProfileRepositoryMock
                .Setup(repository =>
                    repository.UpdateAsync(
                        It.IsAny<CareerProfile>()))
                .ReturnsAsync(
                    (CareerProfile profile) => profile);

            // Act
            var result =
                await _careerProfileService.UpdateAsync(
                    userId,
                    request);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                "Senior Software Engineer",
                result.CurrentJobTitle);

            Assert.Equal(
                3,
                result.YearsOfExperience);

            Assert.Equal(
                "Angular, ASP.NET Core",
                result.Skills);

            Assert.Equal(
                "Coimbatore",
                result.CurrentLocation);

            Assert.Equal(
                "Remote, Bangalore",
                result.PreferredLocations);

            Assert.Equal(
                "Full Stack Engineer",
                result.TargetRole);

            Assert.Equal(
                2000000,
                result.TargetSalary);

            Assert.Equal(
                "Updated summary",
                result.ProfessionalSummary);

            Assert.True(
                result.UpdatedAt > originalUpdatedAt);

            _careerProfileRepositoryMock.Verify(
                repository =>
                    repository.UpdateAsync(
                        It.Is<CareerProfile>(profile =>
                            profile.UserId == userId &&
                            profile.CurrentJobTitle ==
                                "Senior Software Engineer" &&
                            profile.Skills ==
                                "Angular, ASP.NET Core" &&
                            profile.PreferredLocations ==
                                "Remote, Bangalore" &&
                            profile.TargetRole ==
                                "Full Stack Engineer" &&
                            profile.ProfessionalSummary ==
                                "Updated summary")),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenProfileDoesNotExist_ReturnsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new UpdateCareerProfileDto
            {
                CurrentJobTitle = "Software Engineer",
                YearsOfExperience = 2,
                Skills = "Angular, ASP.NET Core",
                CurrentLocation = "Coimbatore",
                PreferredLocations = "Remote",
                TargetRole = "Full Stack Engineer",
                TargetSalary = 2000000,
                ProfessionalSummary = "Professional summary"
            };

            _careerProfileRepositoryMock
                .Setup(repository =>
                    repository.GetByUserIdAsync(userId))
                .ReturnsAsync((CareerProfile?)null);

            // Act
            var result =
                await _careerProfileService.UpdateAsync(
                    userId,
                    request);

            // Assert
            Assert.Null(result);

            _careerProfileRepositoryMock.Verify(
                repository =>
                    repository.UpdateAsync(
                        It.IsAny<CareerProfile>()),
                Times.Never);
        }
    }
}