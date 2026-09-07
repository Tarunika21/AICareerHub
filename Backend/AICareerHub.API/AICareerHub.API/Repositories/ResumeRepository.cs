using AICareerHub.API.Data;
using AICareerHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AICareerHub.API.Repositories
{
    public class ResumeRepository : IResumeRepository
    {
        private readonly ApplicationDbContext _context;

        public ResumeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Resume>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Resumes
                .Where(resume => resume.UserId == userId)
                .OrderByDescending(resume => resume.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Resume?> GetByIdAndUserIdAsync(
            Guid resumeId,
            Guid userId)
        {
            return await _context.Resumes
                .FirstOrDefaultAsync(
                    resume =>
                        resume.Id == resumeId &&
                        resume.UserId == userId);
        }

        public async Task<Resume> CreateAsync(Resume resume)
        {
            _context.Resumes.Add(resume);

            await _context.SaveChangesAsync();

            return resume;
        }

        public async Task<Resume> UpdateAsync(Resume resume)
        {
            _context.Resumes.Update(resume);

            await _context.SaveChangesAsync();

            return resume;
        }

        public async Task DeleteAsync(Resume resume)
        {
            _context.Resumes.Remove(resume);

            await _context.SaveChangesAsync();
        }
    }
}