using AICareerHub.API.Models;

namespace AICareerHub.API.Repositories
{
    public interface IJobApplicationRepository
    {
        Task<IEnumerable<JobApplication>> GetByUserIdAsync(Guid userId);

        Task<JobApplication?> GetByIdAndUserIdAsync(
            Guid jobApplicationId,
            Guid userId);

        Task<JobApplication> CreateAsync(JobApplication jobApplication);

        Task<JobApplication> UpdateAsync(JobApplication jobApplication);

        Task DeleteAsync(JobApplication jobApplication);

        Task<IEnumerable<JobApplication>> SearchAsync(
            Guid userId,
            string? status,
            string? search);
    }
}