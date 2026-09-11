using AICareerHub.API.Data;
using AICareerHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AICareerHub.API.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public JobApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobApplication>> GetByUserIdAsync(
            Guid userId)
        {
            return await _context.JobApplications
                .Where(job => job.UserId == userId)
                .OrderByDescending(job => job.AppliedDate)
                .ToListAsync();
        }

        public async Task<JobApplication?> GetByIdAndUserIdAsync(
            Guid jobApplicationId,
            Guid userId)
        {
            return await _context.JobApplications
                .FirstOrDefaultAsync(job =>
                    job.Id == jobApplicationId &&
                    job.UserId == userId);
        }

        public async Task<JobApplication> CreateAsync(
            JobApplication jobApplication)
        {
            _context.JobApplications.Add(jobApplication);

            await _context.SaveChangesAsync();

            return jobApplication;
        }

        public async Task<JobApplication> UpdateAsync(
            JobApplication jobApplication)
        {
            _context.JobApplications.Update(jobApplication);

            await _context.SaveChangesAsync();

            return jobApplication;
        }

        public async Task DeleteAsync(JobApplication jobApplication)
        {
            _context.JobApplications.Remove(jobApplication);

            await _context.SaveChangesAsync();
        }
    }
}