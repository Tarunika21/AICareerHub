using AICareerHub.API.DTOs;
using AICareerHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AICareerHub.API.Controllers
{
    [ApiController]
    [Route("api/resumes/{resumeId:guid}/experiences")]
    [Authorize]
    public class ResumeExperiencesController : ControllerBase
    {
        private readonly IResumeExperienceService _experienceService;

        public ResumeExperiencesController(
            IResumeExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResumeExperienceDto>>> GetAll(
            Guid resumeId)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var experiences =
                await _experienceService.GetAllAsync(
                    resumeId,
                    userId);

            if (experiences == null)
            {
                return NotFound();
            }

            return Ok(experiences);
        }

        [HttpGet("{experienceId:guid}")]
        public async Task<ActionResult<ResumeExperienceDto>> GetById(
            Guid resumeId,
            Guid experienceId)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var experience =
                await _experienceService.GetByIdAsync(
                    resumeId,
                    experienceId,
                    userId);

            if (experience == null)
            {
                return NotFound();
            }

            return Ok(experience);
        }

        [HttpPost]
        public async Task<ActionResult<ResumeExperienceDto>> Create(
            Guid resumeId,
            CreateResumeExperienceDto createDto)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var createdExperience =
                await _experienceService.CreateAsync(
                    resumeId,
                    userId,
                    createDto);

            if (createdExperience == null)
            {
                return NotFound();
            }

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    resumeId,
                    experienceId = createdExperience.Id
                },
                createdExperience);
        }

        [HttpPut("{experienceId:guid}")]
        public async Task<ActionResult<ResumeExperienceDto>> Update(
            Guid resumeId,
            Guid experienceId,
            UpdateResumeExperienceDto updateDto)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var updatedExperience =
                await _experienceService.UpdateAsync(
                    resumeId,
                    experienceId,
                    userId,
                    updateDto);

            if (updatedExperience == null)
            {
                return NotFound();
            }

            return Ok(updatedExperience);
        }

        [HttpDelete("{experienceId:guid}")]
        public async Task<IActionResult> Delete(
            Guid resumeId,
            Guid experienceId)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var deleted =
                await _experienceService.DeleteAsync(
                    resumeId,
                    experienceId,
                    userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}