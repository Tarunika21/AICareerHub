using AICareerHub.API.Models;

namespace AICareerHub.API.Repositories
{
    public interface IResumeEducationRepository
    {
        Task<IEnumerable<ResumeEducation>> GetByResumeIdAsync(Guid resumeId);

        Task<ResumeEducation?> GetByIdAsync(
            Guid educationId,
            Guid resumeId);

        Task<ResumeEducation> CreateAsync(
            ResumeEducation education);

        Task<ResumeEducation> UpdateAsync(
            ResumeEducation education);

        Task DeleteAsync(
            ResumeEducation education);
    }
}