using AICareerHub.API.Common.Exceptions;
using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;

namespace AICareerHub.API.Services
{
    public class CareerProfileService : ICareerProfileService
    {
        private readonly ICareerProfileRepository _careerProfileRepository;

        public CareerProfileService(
            ICareerProfileRepository careerProfileRepository)
        {
            _careerProfileRepository = careerProfileRepository;
        }

        public async Task<CareerProfileDto?> GetByUserIdAsync(Guid userId)
        {
            var profile =
                await _careerProfileRepository.GetByUserIdAsync(userId);

            if (profile == null)
            {
                return null;
            }

            return MapToDto(profile);
        }

        public async Task<CareerProfileDto> CreateAsync(
            Guid userId,
            CreateCareerProfileDto createDto)
        {
            var existingProfile =
                await _careerProfileRepository.GetByUserIdAsync(userId);

            if (existingProfile != null)
            {
                throw new ConflictException(
                    "Career profile already exists.");
            }

            var profile = new CareerProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CurrentJobTitle = createDto.CurrentJobTitle.Trim(),
                YearsOfExperience = createDto.YearsOfExperience,
                Skills = createDto.Skills.Trim(),
                CurrentLocation = createDto.CurrentLocation.Trim(),
                PreferredLocations = createDto.PreferredLocations.Trim(),
                TargetRole = createDto.TargetRole.Trim(),
                TargetSalary = createDto.TargetSalary,
                ProfessionalSummary =
                    createDto.ProfessionalSummary?.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdProfile =
                await _careerProfileRepository.CreateAsync(profile);

            return MapToDto(createdProfile);
        }

        public async Task<CareerProfileDto?> UpdateAsync(
            Guid userId,
            UpdateCareerProfileDto updateDto)
        {
            var profile =
                await _careerProfileRepository.GetByUserIdAsync(userId);

            if (profile == null)
            {
                return null;
            }

            profile.CurrentJobTitle =
                updateDto.CurrentJobTitle.Trim();

            profile.YearsOfExperience =
                updateDto.YearsOfExperience;

            profile.Skills =
                updateDto.Skills.Trim();

            profile.CurrentLocation =
                updateDto.CurrentLocation.Trim();

            profile.PreferredLocations =
                updateDto.PreferredLocations.Trim();

            profile.TargetRole =
                updateDto.TargetRole.Trim();

            profile.TargetSalary =
                updateDto.TargetSalary;

            profile.ProfessionalSummary =
                updateDto.ProfessionalSummary?.Trim();

            profile.UpdatedAt = DateTime.UtcNow;

            var updatedProfile =
                await _careerProfileRepository.UpdateAsync(profile);

            return MapToDto(updatedProfile);
        }

        private static CareerProfileDto MapToDto(
            CareerProfile profile)
        {
            return new CareerProfileDto
            {
                Id = profile.Id,
                CurrentJobTitle = profile.CurrentJobTitle,
                YearsOfExperience = profile.YearsOfExperience,
                Skills = profile.Skills,
                CurrentLocation = profile.CurrentLocation,
                PreferredLocations = profile.PreferredLocations,
                TargetRole = profile.TargetRole,
                TargetSalary = profile.TargetSalary,
                ProfessionalSummary = profile.ProfessionalSummary,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }
    }
}