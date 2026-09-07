using AICareerHub.API.Data;
using AICareerHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AICareerHub.API.Repositories
{
    public class ResumeExperienceRepository
        : IResumeExperienceRepository
    {
        private readonly ApplicationDbContext _context;

        public ResumeExperienceRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResumeExperience>>
            GetByResumeIdAsync(Guid resumeId)
        {
            return await _context.ResumeExperiences
                .Where(experience =>
                    experience.ResumeId == resumeId)
                .OrderByDescending(experience =>
                    experience.StartDate)
                .ToListAsync();
        }

        public async Task<ResumeExperience?> GetByIdAsync(
            Guid experienceId,
            Guid resumeId)
        {
            return await _context.ResumeExperiences
                .FirstOrDefaultAsync(experience =>
                    experience.Id == experienceId &&
                    experience.ResumeId == resumeId);
        }

        public async Task<ResumeExperience> CreateAsync(
            ResumeExperience experience)
        {
            _context.ResumeExperiences.Add(experience);

            await _context.SaveChangesAsync();

            return experience;
        }

        public async Task<ResumeExperience> UpdateAsync(
            ResumeExperience experience)
        {
            _context.ResumeExperiences.Update(experience);

            await _context.SaveChangesAsync();

            return experience;
        }

        public async Task DeleteAsync(
            ResumeExperience experience)
        {
            _context.ResumeExperiences.Remove(experience);

            await _context.SaveChangesAsync();
        }
    }
}