using AICareerHub.API.DTOs;
using AICareerHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AICareerHub.API.Controllers
{
    [ApiController]
    [Route("api/resumes")]
    [Authorize]
    public class ResumesController : ControllerBase
    {
        private readonly IResumeService _resumeService;

        public ResumesController(IResumeService resumeService)
        {
            _resumeService = resumeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResumeDto>>> GetAll()
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var resumes =
                await _resumeService.GetAllAsync(userId);

            return Ok(resumes);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResumeDto>> GetById(Guid id)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var resume =
                await _resumeService.GetByIdAsync(id, userId);

            if (resume == null)
            {
                return NotFound();
            }

            return Ok(resume);
        }

        [HttpPost]
        public async Task<ActionResult<ResumeDto>> Create(
            CreateResumeDto createDto)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var createdResume =
                await _resumeService.CreateAsync(
                    userId,
                    createDto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdResume.Id },
                createdResume);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ResumeDto>> Update(
            Guid id,
            UpdateResumeDto updateDto)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var updatedResume =
                await _resumeService.UpdateAsync(
                    id,
                    userId,
                    updateDto);

            if (updatedResume == null)
            {
                return NotFound();
            }

            return Ok(updatedResume);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var deleted =
                await _resumeService.DeleteAsync(
                    id,
                    userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}