using AICareerHub.API.DTOs;

namespace AICareerHub.API.Services
{
    public interface IResumeEducationService
    {
        Task<IEnumerable<ResumeEducationDto>?> GetAllAsync(
            Guid resumeId,
            Guid userId);

        Task<ResumeEducationDto?> GetByIdAsync(
            Guid resumeId,
            Guid educationId,
            Guid userId);

        Task<ResumeEducationDto?> CreateAsync(
            Guid resumeId,
            Guid userId,
            CreateResumeEducationDto createDto);

        Task<ResumeEducationDto?> UpdateAsync(
            Guid resumeId,
            Guid educationId,
            Guid userId,
            UpdateResumeEducationDto updateDto);

        Task<bool> DeleteAsync(
            Guid resumeId,
            Guid educationId,
            Guid userId);
    }
}