using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;

namespace AICareerHub.API.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IResumeRepository _resumeRepository;

        public ResumeService(IResumeRepository resumeRepository)
        {
            _resumeRepository = resumeRepository;
        }

        public async Task<IEnumerable<ResumeDto>> GetAllAsync(Guid userId)
        {
            var resumes =
                await _resumeRepository.GetByUserIdAsync(userId);

            return resumes.Select(MapToDto);
        }

        public async Task<ResumeDto?> GetByIdAsync(
            Guid resumeId,
            Guid userId)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            return resume == null
                ? null
                : MapToDto(resume);
        }

        public async Task<ResumeDto> CreateAsync(
            Guid userId,
            CreateResumeDto createDto)
        {
            var resume = new Resume
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = createDto.Title.Trim(),
                ProfessionalSummary =
                    createDto.ProfessionalSummary?.Trim(),
                Skills = createDto.Skills.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdResume =
                await _resumeRepository.CreateAsync(resume);

            return MapToDto(createdResume);
        }

        public async Task<ResumeDto?> UpdateAsync(
            Guid resumeId,
            Guid userId,
            UpdateResumeDto updateDto)
        {
            var resume =
                await _resumeRepository.GetByIdAndUserIdAsync(
                    resumeId,
                    userId);

            if (resume == null)
            {
                return null;
            }

            resume.Title = updateDto.Title.Trim();

            resume.ProfessionalSummary =
                updateDto.ProfessionalSummary?.Trim();

            resume.Skills =
                updateDto.Skills.Trim();

            resume.UpdatedAt = DateTime.UtcNow;

            var updatedResume =
                await _resumeRepository.UpdateAsync(resume);

            return MapToDto(updatedResume);
        }

        public async Task<bool> DeleteAsync(
            Guid resumeId,
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

            await _resumeRepository.DeleteAsync(resume);

            return true;
        }

        private static ResumeDto MapToDto(Resume resume)
        {
            return new ResumeDto
            {
                Id = resume.Id,
                Title = resume.Title,
                ProfessionalSummary =
                    resume.ProfessionalSummary,
                Skills = resume.Skills,
                CreatedAt = resume.CreatedAt,
                UpdatedAt = resume.UpdatedAt
            };
        }
    }
}