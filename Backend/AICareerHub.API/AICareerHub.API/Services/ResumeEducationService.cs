using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;

namespace AICareerHub.API.Services
{
    public class ResumeEducationService
        : IResumeEducationService
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IResumeEducationRepository _educationRepository;

        public ResumeEducationService(
            IResumeRepository resumeRepository,
            IResumeEducationRepository educationRepository)
        {
            _resumeRepository = resumeRepository;
            _educationRepository = educationRepository;
        }

        public async Task<IEnumerable<ResumeEducationDto>?> GetAllAsync(
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

            var educations =
                await _educationRepository.GetByResumeIdAsync(
                    resumeId);

            return educations.Select(MapToDto);
        }

        public async Task<ResumeEducationDto?> GetByIdAsync(
            Guid resumeId,
            Guid educationId,
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

            var education =
                await _educationRepository.GetByIdAsync(
                    educationId,
                    resumeId);

            return education == null
                ? null
                : MapToDto(education);
        }

        public async Task<ResumeEducationDto?> CreateAsync(
            Guid resumeId,
            Guid userId,
            CreateResumeEducationDto createDto)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return null;
            }

            ValidateYears(
                createDto.StartYear,
                createDto.EndYear);

            var education = new ResumeEducation
            {
                Id = Guid.NewGuid(),
                ResumeId = resumeId,
                Institution = createDto.Institution.Trim(),
                Degree = createDto.Degree.Trim(),
                FieldOfStudy = createDto.FieldOfStudy?.Trim(),
                StartYear = createDto.StartYear,
                EndYear = createDto.EndYear
            };

            var createdEducation =
                await _educationRepository.CreateAsync(
                    education);

            return MapToDto(createdEducation);
        }

        public async Task<ResumeEducationDto?> UpdateAsync(
            Guid resumeId,
            Guid educationId,
            Guid userId,
            UpdateResumeEducationDto updateDto)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return null;
            }

            var education =
                await _educationRepository.GetByIdAsync(
                    educationId,
                    resumeId);

            if (education == null)
            {
                return null;
            }

            ValidateYears(
                updateDto.StartYear,
                updateDto.EndYear);

            education.Institution =
                updateDto.Institution.Trim();

            education.Degree =
                updateDto.Degree.Trim();

            education.FieldOfStudy =
                updateDto.FieldOfStudy?.Trim();

            education.StartYear =
                updateDto.StartYear;

            education.EndYear =
                updateDto.EndYear;

            var updatedEducation =
                await _educationRepository.UpdateAsync(
                    education);

            return MapToDto(updatedEducation);
        }

        public async Task<bool> DeleteAsync(
            Guid resumeId,
            Guid educationId,
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

            var education =
                await _educationRepository.GetByIdAsync(
                    educationId,
                    resumeId);

            if (education == null)
            {
                return false;
            }

            await _educationRepository.DeleteAsync(
                education);

            return true;
        }

        private static void ValidateYears(
            int startYear,
            int? endYear)
        {
            if (endYear.HasValue &&
                endYear.Value < startYear)
            {
                throw new ArgumentException(
                    "End year cannot be earlier than start year.");
            }
        }

        private static ResumeEducationDto MapToDto(
            ResumeEducation education)
        {
            return new ResumeEducationDto
            {
                Id = education.Id,
                Institution = education.Institution,
                Degree = education.Degree,
                FieldOfStudy = education.FieldOfStudy,
                StartYear = education.StartYear,
                EndYear = education.EndYear
            };
        }
    }
}