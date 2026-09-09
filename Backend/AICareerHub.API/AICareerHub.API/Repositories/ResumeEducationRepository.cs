using AICareerHub.API.Data;
using AICareerHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AICareerHub.API.Repositories
{
    public class ResumeEducationRepository
        : IResumeEducationRepository
    {
        private readonly ApplicationDbContext _context;

        public ResumeEducationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResumeEducation>>
            GetByResumeIdAsync(Guid resumeId)
        {
            return await _context.ResumeEducations
                .Where(education =>
                    education.ResumeId == resumeId)
                .OrderByDescending(education =>
                    education.StartYear)
                .ToListAsync();
        }

        public async Task<ResumeEducation?> GetByIdAsync(
            Guid educationId,
            Guid resumeId)
        {
            return await _context.ResumeEducations
                .FirstOrDefaultAsync(education =>
                    education.Id == educationId &&
                    education.ResumeId == resumeId);
        }

        public async Task<ResumeEducation> CreateAsync(
            ResumeEducation education)
        {
            _context.ResumeEducations.Add(education);

            await _context.SaveChangesAsync();

            return education;
        }

        public async Task<ResumeEducation> UpdateAsync(
            ResumeEducation education)
        {
            _context.ResumeEducations.Update(education);

            await _context.SaveChangesAsync();

            return education;
        }

        public async Task DeleteAsync(
            ResumeEducation education)
        {
            _context.ResumeEducations.Remove(education);

            await _context.SaveChangesAsync();
        }
    }
}