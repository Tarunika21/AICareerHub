using AICareerHub.API.DTOs;
using AICareerHub.API.Models;
using AICareerHub.API.Repositories;

namespace AICareerHub.API.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _repository;

        private static readonly HashSet<string> AllowedStatuses =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "Applied",
                "Interview",
                "Offer",
                "Rejected",
                "Withdrawn"
            };

        public JobApplicationService(
            IJobApplicationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<JobApplicationDto>>
            GetAllAsync(Guid userId)
        {
            var jobs = await _repository.GetByUserIdAsync(userId);

            return jobs.Select(MapToDto);
        }

        public async Task<JobApplicationDto?> GetByIdAsync(
            Guid jobApplicationId,
            Guid userId)
        {
            var job =
                await _repository.GetByIdAndUserIdAsync(
                    jobApplicationId,
                    userId);

            return job == null ? null : MapToDto(job);
        }

        public async Task<JobApplicationDto> CreateAsync(
            Guid userId,
            CreateJobApplicationDto createDto)
        {
            ValidateStatus(createDto.Status);

            var now = DateTime.UtcNow;

            var job = new JobApplication
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CompanyName = createDto.CompanyName.Trim(),
                JobTitle = createDto.JobTitle.Trim(),
                JobUrl = createDto.JobUrl?.Trim(),
                Location = createDto.Location.Trim(),
                Status = NormalizeStatus(createDto.Status),
                AppliedDate = createDto.AppliedDate,
                Notes = createDto.Notes?.Trim(),
                CreatedAt = now,
                UpdatedAt = now
            };

            var created = await _repository.CreateAsync(job);

            return MapToDto(created);
        }

        public async Task<JobApplicationDto?> UpdateAsync(
            Guid jobApplicationId,
            Guid userId,
            UpdateJobApplicationDto updateDto)
        {
            ValidateStatus(updateDto.Status);

            var job =
                await _repository.GetByIdAndUserIdAsync(
                    jobApplicationId,
                    userId);

            if (job == null)
            {
                return null;
            }

            job.CompanyName = updateDto.CompanyName.Trim();
            job.JobTitle = updateDto.JobTitle.Trim();
            job.JobUrl = updateDto.JobUrl?.Trim();
            job.Location = updateDto.Location.Trim();
            job.Status = NormalizeStatus(updateDto.Status);
            job.AppliedDate = updateDto.AppliedDate;
            job.Notes = updateDto.Notes?.Trim();
            job.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(job);

            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(
            Guid jobApplicationId,
            Guid userId)
        {
            var job =
                await _repository.GetByIdAndUserIdAsync(
                    jobApplicationId,
                    userId);

            if (job == null)
            {
                return false;
            }

            await _repository.DeleteAsync(job);

            return true;
        }

        private static void ValidateStatus(string status)
        {
            if (!AllowedStatuses.Contains(status.Trim()))
            {
                throw new ArgumentException(
                    "Status must be Applied, Interview, Offer, Rejected, or Withdrawn.");
            }
        }

        private static string NormalizeStatus(string status)
        {
            return AllowedStatuses
                .First(s =>
                    s.Equals(
                        status.Trim(),
                        StringComparison.OrdinalIgnoreCase));
        }

        private static JobApplicationDto MapToDto(
            JobApplication job)
        {
            return new JobApplicationDto
            {
                Id = job.Id,
                CompanyName = job.CompanyName,
                JobTitle = job.JobTitle,
                JobUrl = job.JobUrl,
                Location = job.Location,
                Status = job.Status,
                AppliedDate = job.AppliedDate,
                Notes = job.Notes,
                CreatedAt = job.CreatedAt,
                UpdatedAt = job.UpdatedAt
            };
        }
    }
}