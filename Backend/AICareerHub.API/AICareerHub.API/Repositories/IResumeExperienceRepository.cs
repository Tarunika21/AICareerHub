using AICareerHub.API.Models;

namespace AICareerHub.API.Repositories
{
    public interface IResumeExperienceRepository
    {
        Task<IEnumerable<ResumeExperience>> GetByResumeIdAsync(Guid resumeId);

        Task<ResumeExperience?> GetByIdAsync(
            Guid experienceId,
            Guid resumeId);

        Task<ResumeExperience> CreateAsync(
            ResumeExperience experience);

        Task<ResumeExperience> UpdateAsync(
            ResumeExperience experience);

        Task DeleteAsync(
            ResumeExperience experience);
    }
}