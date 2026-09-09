using AICareerHub.API.DTOs;

namespace AICareerHub.API.Services
{
    public interface IResumeProjectService
    {
        Task<IEnumerable<ResumeProjectDto>?> GetAllAsync(
            Guid resumeId,
            Guid userId);

        Task<ResumeProjectDto?> GetByIdAsync(
            Guid resumeId,
            Guid projectId,
            Guid userId);

        Task<ResumeProjectDto?> CreateAsync(
            Guid resumeId,
            Guid userId,
            CreateResumeProjectDto createDto);

        Task<ResumeProjectDto?> UpdateAsync(
            Guid resumeId,
            Guid projectId,
            Guid userId,
            UpdateResumeProjectDto updateDto);

        Task<bool> DeleteAsync(
            Guid resumeId,
            Guid projectId,
            Guid userId);
    }
}