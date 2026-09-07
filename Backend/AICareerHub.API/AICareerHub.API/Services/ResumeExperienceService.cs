using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;

namespace AICareerHub.API.Services
{
    public class ResumeExperienceService
        : IResumeExperienceService
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IResumeExperienceRepository _experienceRepository;

        public ResumeExperienceService(
            IResumeRepository resumeRepository,
            IResumeExperienceRepository experienceRepository)
        {
            _resumeRepository = resumeRepository;
            _experienceRepository = experienceRepository;
        }

        public async Task<IEnumerable<ResumeExperienceDto>?> GetAllAsync(
            Guid resumeId,
            Guid userId)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return null;
            }

            var experiences =
                await _experienceRepository.GetByResumeIdAsync(
                    resumeId);

            return experiences.Select(MapToDto);
        }

        public async Task<ResumeExperienceDto?> GetByIdAsync(
            Guid resumeId,
            Guid experienceId,
            Guid userId)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return null;
            }

            var experience =
                await _experienceRepository.GetByIdAsync(
                    experienceId,
                    resumeId);

            return experience == null
                ? null
                : MapToDto(experience);
        }

        public async Task<ResumeExperienceDto?> CreateAsync(
            Guid resumeId,
            Guid userId,
            CreateResumeExperienceDto createDto)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return null;
            }

            ValidateDates(
                createDto.StartDate,
                createDto.EndDate,
                createDto.IsCurrent);

            var experience = new ResumeExperience
            {
                Id = Guid.NewGuid(),
                ResumeId = resumeId,
                Company = createDto.Company.Trim(),
                JobTitle = createDto.JobTitle.Trim(),
                StartDate = createDto.StartDate,
                EndDate = createDto.IsCurrent
                    ? null
                    : createDto.EndDate,
                IsCurrent = createDto.IsCurrent,
                Description = createDto.Description?.Trim()
            };

            var createdExperience =
                await _experienceRepository.CreateAsync(
                    experience);

            return MapToDto(createdExperience);
        }

        public async Task<ResumeExperienceDto?> UpdateAsync(
            Guid resumeId,
            Guid experienceId,
            Guid userId,
            UpdateResumeExperienceDto updateDto)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return null;
            }

            var experience =
                await _experienceRepository.GetByIdAsync(
                    experienceId,
                    resumeId);

            if (experience == null)
            {
                return null;
            }

            ValidateDates(
                updateDto.StartDate,
                updateDto.EndDate,
                updateDto.IsCurrent);

            experience.Company =
                updateDto.Company.Trim();

            experience.JobTitle =
                updateDto.JobTitle.Trim();

            experience.StartDate =
                updateDto.StartDate;

            experience.EndDate =
                updateDto.IsCurrent
                    ? null
                    : updateDto.EndDate;

            experience.IsCurrent =
                updateDto.IsCurrent;

            experience.Description =
                updateDto.Description?.Trim();

            var updatedExperience =
                await _experienceRepository.UpdateAsync(
                    experience);

            return MapToDto(updatedExperience);
        }

        public async Task<bool> DeleteAsync(
            Guid resumeId,
            Guid experienceId,
            Guid userId)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return false;
            }

            var experience =
                await _experienceRepository.GetByIdAsync(
                    experienceId,
                    resumeId);

            if (experience == null)
            {
                return false;
            }

            await _experienceRepository.DeleteAsync(
                experience);

            return true;
        }

        private static void ValidateDates(
            DateOnly startDate,
            DateOnly? endDate,
            bool isCurrent)
        {
            if (!isCurrent &&
                endDate.HasValue &&
                endDate.Value < startDate)
            {
                throw new ArgumentException(
                    "End date cannot be earlier than start date.");
            }
        }

        private static ResumeExperienceDto MapToDto(
            ResumeExperience experience)
        {
            return new ResumeExperienceDto
            {
                Id = experience.Id,
                Company = experience.Company,
                JobTitle = experience.JobTitle,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                IsCurrent = experience.IsCurrent,
                Description = experience.Description
            };
        }
    }
}