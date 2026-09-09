using AICareerHub.API.Models;

namespace AICareerHub.API.Repositories
{
    public interface IResumeProjectRepository
    {
        Task<IEnumerable<ResumeProject>> GetByResumeIdAsync(Guid resumeId);

        Task<ResumeProject?> GetByIdAsync(
            Guid projectId,
            Guid resumeId);

        Task<ResumeProject> CreateAsync(
            ResumeProject project);

        Task<ResumeProject> UpdateAsync(
            ResumeProject project);

        Task DeleteAsync(
            ResumeProject project);
    }
}