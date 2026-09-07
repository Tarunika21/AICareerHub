using AICareerHub.API.DTOs;

namespace AICareerHub.API.Services
{
    public interface IResumeExperienceService
    {
        Task<IEnumerable<ResumeExperienceDto>?> GetAllAsync(
            Guid resumeId,
            Guid userId);

        Task<ResumeExperienceDto?> GetByIdAsync(
            Guid resumeId,
            Guid experienceId,
            Guid userId);

        Task<ResumeExperienceDto?> CreateAsync(
            Guid resumeId,
            Guid userId,
            CreateResumeExperienceDto createDto);

        Task<ResumeExperienceDto?> UpdateAsync(
            Guid resumeId,
            Guid experienceId,
            Guid userId,
            UpdateResumeExperienceDto updateDto);

        Task<bool> DeleteAsync(
            Guid resumeId,
            Guid experienceId,
            Guid userId);
    }
}