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

        public async Task<IEnumerable<JobApplication>> SearchAsync(
            Guid userId,
            string? status,
            string? search)
        {
            var query = _context.JobApplications
                .Where(job => job.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(job =>
                    job.Status.ToLower() == status.Trim().ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchValue = search.Trim().ToLower();

                query = query.Where(job =>
                    job.CompanyName.ToLower().Contains(searchValue) ||
                    job.JobTitle.ToLower().Contains(searchValue) ||
                    job.Location.ToLower().Contains(searchValue));
            }

            return await query
                .OrderByDescending(job => job.AppliedDate)
                .ToListAsync();
        }
    }
}