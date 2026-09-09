using AICareerHub.API.DTOs;
using AICareerHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AICareerHub.API.Controllers
{
    [ApiController]
    [Route("api/resumes/{resumeId:guid}/educations")]
    [Authorize]
    public class ResumeEducationsController : ControllerBase
    {
        private readonly IResumeEducationService _educationService;

        public ResumeEducationsController(
            IResumeEducationService educationService)
        {
            _educationService = educationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResumeEducationDto>>> GetAll(
            Guid resumeId)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var educations =
                await _educationService.GetAllAsync(
                    resumeId,
                    userId);

            if (educations == null)
            {
                return NotFound();
            }

            return Ok(educations);
        }

        [HttpGet("{educationId:guid}")]
        public async Task<ActionResult<ResumeEducationDto>> GetById(
            Guid resumeId,
            Guid educationId)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var education =
                await _educationService.GetByIdAsync(
                    resumeId,
                    educationId,
                    userId);

            if (education == null)
            {
                return NotFound();
            }

            return Ok(education);
        }

        [HttpPost]
        public async Task<ActionResult<ResumeEducationDto>> Create(
            Guid resumeId,
            CreateResumeEducationDto createDto)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var createdEducation =
                await _educationService.CreateAsync(
                    resumeId,
                    userId,
                    createDto);

            if (createdEducation == null)
            {
                return NotFound();
            }

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    resumeId,
                    educationId = createdEducation.Id
                },
                createdEducation);
        }

        [HttpPut("{educationId:guid}")]
        public async Task<ActionResult<ResumeEducationDto>> Update(
            Guid resumeId,
            Guid educationId,
            UpdateResumeEducationDto updateDto)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var updatedEducation =
                await _educationService.UpdateAsync(
                    resumeId,
                    educationId,
                    userId,
                    updateDto);

            if (updatedEducation == null)
            {
                return NotFound();
            }

            return Ok(updatedEducation);
        }

        [HttpDelete("{educationId:guid}")]
        public async Task<IActionResult> Delete(
            Guid resumeId,
            Guid educationId)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var deleted =
                await _educationService.DeleteAsync(
                    resumeId,
                    educationId,
                    userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}