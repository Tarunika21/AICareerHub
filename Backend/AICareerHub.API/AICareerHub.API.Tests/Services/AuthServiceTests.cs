using AICareerHub.API.Common.Exceptions;
using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;
using AICareerHub.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AICareerHub.API.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock =
                new Mock<IUserRepository>();

            _passwordHasherMock =
                new Mock<IPasswordHasher<User>>();

            var configurationValues =
                new Dictionary<string, string?>
                {
                    ["Jwt:Key"] =
                        "ThisIsATestJwtKeyThatIsLongEnoughForTesting123456",
                    ["Jwt:Issuer"] = "AICareerHub",
                    ["Jwt:Audience"] = "AICareerHubUsers",
                    ["Jwt:ExpiryMinutes"] = "60"
                };

            _configuration =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(configurationValues)
                    .Build();

            _authService =
                new AuthService(
                    _userRepositoryMock.Object,
                    _passwordHasherMock.Object,
                    _configuration);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailDoesNotExist_CreatesAndReturnsUser()
        {
            // Arrange
            var request = new RegisterDto
            {
                FirstName = " Tarunika ",
                LastName = " V ",
                Email = " TARUNIKA@EXAMPLE.COM ",
                Password = "Password123"
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.EmailExistsAsync("tarunika@example.com"))
                .ReturnsAsync(false);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.HashPassword(
                        It.IsAny<User>(),
                        request.Password))
                .Returns("hashed-password");

            _userRepositoryMock
                .Setup(repository =>
                    repository.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) => user);

            // Act
            var result =
                await _authService.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Tarunika", result.FirstName);
            Assert.Equal("V", result.LastName);
            Assert.Equal("tarunika@example.com", result.Email);

            _userRepositoryMock.Verify(
                repository =>
                    repository.EmailExistsAsync(
                        "tarunika@example.com"),
                Times.Once);

            _passwordHasherMock.Verify(
                hasher =>
                    hasher.HashPassword(
                        It.Is<User>(user =>
                            user.Email == "tarunika@example.com" &&
                            user.FirstName == "Tarunika" &&
                            user.LastName == "V"),
                        "Password123"),
                Times.Once);

            _userRepositoryMock.Verify(
                repository =>
                    repository.CreateAsync(
                        It.Is<User>(user =>
                            user.Email == "tarunika@example.com" &&
                            user.FirstName == "Tarunika" &&
                            user.LastName == "V" &&
                            user.PasswordHash == "hashed-password")),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsConflictException()
        {
            // Arrange
            var request = new RegisterDto
            {
                FirstName = "Tarunika",
                LastName = "V",
                Email = "tarunika@example.com",
                Password = "Password123"
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.EmailExistsAsync("tarunika@example.com"))
                .ReturnsAsync(true);

            // Act
            var exception = await Assert.ThrowsAsync<ConflictException>(
                () => _authService.RegisterAsync(request));

            // Assert
            Assert.Equal(
                "Email address is already registered.",
                exception.Message);

            _userRepositoryMock.Verify(
                repository =>
                    repository.CreateAsync(It.IsAny<User>()),
                Times.Never);

            _passwordHasherMock.Verify(
                hasher =>
                    hasher.HashPassword(
                        It.IsAny<User>(),
                        It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsTokenAndUser()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                FirstName = "Tarunika",
                LastName = "V",
                Email = "tarunika@example.com",
                PasswordHash = "hashed-password",
                CreatedAt = DateTime.UtcNow
            };

            var request = new LoginDto
            {
                Email = " TARUNIKA@EXAMPLE.COM ",
                Password = "Password123"
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.GetByEmailAsync(
                        "tarunika@example.com"))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.VerifyHashedPassword(
                        user,
                        "hashed-password",
                        "Password123"))
                .Returns(
                    PasswordVerificationResult.Success);

            // Act
            var result =
                await _authService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.Token));

            Assert.Equal(userId, result.User.Id);
            Assert.Equal("Tarunika", result.User.FirstName);
            Assert.Equal("V", result.User.LastName);
            Assert.Equal(
                "tarunika@example.com",
                result.User.Email);

            Assert.True(
                result.ExpiresAt > DateTime.UtcNow);

            _userRepositoryMock.Verify(
                repository =>
                    repository.GetByEmailAsync(
                        "tarunika@example.com"),
                Times.Once);

            _passwordHasherMock.Verify(
                hasher =>
                    hasher.VerifyHashedPassword(
                        user,
                        "hashed-password",
                        "Password123"),
                Times.Once);
        }

        [Fact]
        public async Task LoginAsync_WhenUserDoesNotExist_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new LoginDto
            {
                Email = "unknown@example.com",
                Password = "Password123"
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.GetByEmailAsync(
                        "unknown@example.com"))
                .ReturnsAsync((User?)null);

            // Act
            var exception =
                await Assert.ThrowsAsync<UnauthorizedAccessException>(
                    () => _authService.LoginAsync(request));

            // Assert
            Assert.Equal(
                "Invalid email or password.",
                exception.Message);

            _passwordHasherMock.Verify(
                hasher =>
                    hasher.VerifyHashedPassword(
                        It.IsAny<User>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIsIncorrect_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Tarunika",
                LastName = "V",
                Email = "tarunika@example.com",
                PasswordHash = "hashed-password",
                CreatedAt = DateTime.UtcNow
            };

            var request = new LoginDto
            {
                Email = "tarunika@example.com",
                Password = "WrongPassword"
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.GetByEmailAsync(
                        "tarunika@example.com"))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.VerifyHashedPassword(
                        user,
                        "hashed-password",
                        "WrongPassword"))
                .Returns(
                    PasswordVerificationResult.Failed);

            // Act
            var exception =
                await Assert.ThrowsAsync<UnauthorizedAccessException>(
                    () => _authService.LoginAsync(request));

            // Assert
            Assert.Equal(
                "Invalid email or password.",
                exception.Message);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_GeneratesTokenWithCorrectClaims()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                FirstName = "Tarunika",
                LastName = "V",
                Email = "tarunika@example.com",
                PasswordHash = "hashed-password",
                CreatedAt = DateTime.UtcNow
            };

            var request = new LoginDto
            {
                Email = "tarunika@example.com",
                Password = "Password123"
            };

            _userRepositoryMock
                .Setup(repository =>
                    repository.GetByEmailAsync(
                        "tarunika@example.com"))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.VerifyHashedPassword(
                        user,
                        "hashed-password",
                        "Password123"))
                .Returns(
                    PasswordVerificationResult.Success);

            // Act
            var result =
                await _authService.LoginAsync(request);

            var tokenHandler =
                new JwtSecurityTokenHandler();

            var token =
                tokenHandler.ReadJwtToken(result.Token);

            // Assert
            Assert.Equal(
                "AICareerHub",
                token.Issuer);

            Assert.Contains(
                "AICareerHubUsers",
                token.Audiences);

            Assert.Contains(
                token.Claims,
                claim =>
                    claim.Type == ClaimTypes.NameIdentifier &&
                    claim.Value == userId.ToString());

            Assert.Contains(
                token.Claims,
                claim =>
                    claim.Type == ClaimTypes.Email &&
                    claim.Value == "tarunika@example.com");

            Assert.Contains(
                token.Claims,
                claim =>
                    claim.Type == ClaimTypes.Name &&
                    claim.Value == "Tarunika V");
        }
    }
}