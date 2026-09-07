using AICareerHub.API.DTOs;

namespace AICareerHub.API.Services
{
    public interface IResumeService
    {
        Task<IEnumerable<ResumeDto>> GetAllAsync(Guid userId);

        Task<ResumeDto?> GetByIdAsync(
            Guid resumeId,
            Guid userId);

        Task<ResumeDto> CreateAsync(
            Guid userId,
            CreateResumeDto createDto);

        Task<ResumeDto?> UpdateAsync(
            Guid resumeId,
            Guid userId,
            UpdateResumeDto updateDto);

        Task<bool> DeleteAsync(
            Guid resumeId,
            Guid userId);
    }
}