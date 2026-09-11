using AICareerHub.API.DTOs;
using AICareerHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AICareerHub.API.Controllers
{
    [ApiController]
    [Route("api/job-applications")]
    [Authorize]
    public class JobApplicationsController : ControllerBase
    {
        private readonly IJobApplicationService _jobApplicationService;

        public JobApplicationsController(
            IJobApplicationService jobApplicationService)
        {
            _jobApplicationService = jobApplicationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobApplicationDto>>> GetAll()
        {
            var userId = GetCurrentUserId();

            var jobs =
                await _jobApplicationService.GetAllAsync(userId);

            return Ok(jobs);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<JobApplicationDto>> GetById(Guid id)
        {
            var userId = GetCurrentUserId();

            var job =
                await _jobApplicationService.GetByIdAsync(id, userId);

            if (job == null)
            {
                return NotFound();
            }

            return Ok(job);
        }

        [HttpPost]
        public async Task<ActionResult<JobApplicationDto>> Create(
            CreateJobApplicationDto createDto)
        {
            var userId = GetCurrentUserId();

            var created =
                await _jobApplicationService.CreateAsync(
                    userId,
                    createDto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<JobApplicationDto>> Update(
            Guid id,
            UpdateJobApplicationDto updateDto)
        {
            var userId = GetCurrentUserId();

            var updated =
                await _jobApplicationService.UpdateAsync(
                    id,
                    userId,
                    updateDto);

            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();

            var deleted =
                await _jobApplicationService.DeleteAsync(
                    id,
                    userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user identity.");
            }

            return userId;
        }
    }
}