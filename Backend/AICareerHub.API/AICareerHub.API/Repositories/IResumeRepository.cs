using AICareerHub.API.Models;

namespace AICareerHub.API.Repositories
{
    public interface IResumeRepository
    {
        Task<IEnumerable<Resume>> GetByUserIdAsync(Guid userId);

        Task<Resume?> GetByIdAndUserIdAsync(
            Guid resumeId,
            Guid userId);

        Task<Resume> CreateAsync(Resume resume);

        Task<Resume> UpdateAsync(Resume resume);

        Task DeleteAsync(Resume resume);
    }
}