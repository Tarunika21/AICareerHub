using AICareerHub.API.DTOs;

namespace AICareerHub.API.Services
{
    public interface IJobApplicationService
    {
        Task<IEnumerable<JobApplicationDto>> GetAllAsync(Guid userId);

        Task<JobApplicationDto?> GetByIdAsync(
            Guid jobApplicationId,
            Guid userId);

        Task<JobApplicationDto> CreateAsync(
            Guid userId,
            CreateJobApplicationDto createDto);

        Task<JobApplicationDto?> UpdateAsync(
            Guid jobApplicationId,
            Guid userId,
            UpdateJobApplicationDto updateDto);

        Task<bool> DeleteAsync(
            Guid jobApplicationId,
            Guid userId);

        Task<IEnumerable<JobApplicationDto>> SearchAsync(
            Guid userId,
            string? status,
            string? search);
    }
}