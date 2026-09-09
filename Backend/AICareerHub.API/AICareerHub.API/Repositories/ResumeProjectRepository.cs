using AICareerHub.API.Data;
using AICareerHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AICareerHub.API.Repositories
{
    public class ResumeProjectRepository
        : IResumeProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ResumeProjectRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResumeProject>>
            GetByResumeIdAsync(Guid resumeId)
        {
            return await _context.ResumeProjects
                .Where(project =>
                    project.ResumeId == resumeId)
                .OrderBy(project =>
                    project.Name)
                .ToListAsync();
        }

        public async Task<ResumeProject?> GetByIdAsync(
            Guid projectId,
            Guid resumeId)
        {
            return await _context.ResumeProjects
                .FirstOrDefaultAsync(project =>
                    project.Id == projectId &&
                    project.ResumeId == resumeId);
        }

        public async Task<ResumeProject> CreateAsync(
            ResumeProject project)
        {
            _context.ResumeProjects.Add(project);

            await _context.SaveChangesAsync();

            return project;
        }

        public async Task<ResumeProject> UpdateAsync(
            ResumeProject project)
        {
            _context.ResumeProjects.Update(project);

            await _context.SaveChangesAsync();

            return project;
        }

        public async Task DeleteAsync(
            ResumeProject project)
        {
            _context.ResumeProjects.Remove(project);

            await _context.SaveChangesAsync();
        }
    }
}