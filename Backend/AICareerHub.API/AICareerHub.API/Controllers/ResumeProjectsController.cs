using AICareerHub.API.DTOs;
using AICareerHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AICareerHub.API.Controllers
{
    [ApiController]
    [Route("api/resumes/{resumeId:guid}/projects")]
    [Authorize]
    public class ResumeProjectsController : ControllerBase
    {
        private readonly IResumeProjectService _projectService;

        public ResumeProjectsController(
            IResumeProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResumeProjectDto>>> GetAll(
            Guid resumeId)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var projects =
                await _projectService.GetAllAsync(
                    resumeId,
                    userId);

            if (projects == null)
            {
                return NotFound();
            }

            return Ok(projects);
        }

        [HttpGet("{projectId:guid}")]
        public async Task<ActionResult<ResumeProjectDto>> GetById(
            Guid resumeId,
            Guid projectId)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var project =
                await _projectService.GetByIdAsync(
                    resumeId,
                    projectId,
                    userId);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<ResumeProjectDto>> Create(
            Guid resumeId,
            CreateResumeProjectDto createDto)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var createdProject =
                await _projectService.CreateAsync(
                    resumeId,
                    userId,
                    createDto);

            if (createdProject == null)
            {
                return NotFound();
            }

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    resumeId,
                    projectId = createdProject.Id
                },
                createdProject);
        }

        [HttpPut("{projectId:guid}")]
        public async Task<ActionResult<ResumeProjectDto>> Update(
            Guid resumeId,
            Guid projectId,
            UpdateResumeProjectDto updateDto)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var updatedProject =
                await _projectService.UpdateAsync(
                    resumeId,
                    projectId,
                    userId,
                    updateDto);

            if (updatedProject == null)
            {
                return NotFound();
            }

            return Ok(updatedProject);
        }

        [HttpDelete("{projectId:guid}")]
        public async Task<IActionResult> Delete(
            Guid resumeId,
            Guid projectId)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var deleted =
                await _projectService.DeleteAsync(
                    resumeId,
                    projectId,
                    userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}