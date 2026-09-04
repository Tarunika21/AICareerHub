using AICareerHub.API.DTOs;
using AICareerHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AICareerHub.API.Controllers
{
    [ApiController]
    [Route("api/career-profile")]
    [Authorize]
    public class CareerProfileController : ControllerBase
    {
        private readonly ICareerProfileService _careerProfileService;

        public CareerProfileController(
            ICareerProfileService careerProfileService)
        {
            _careerProfileService = careerProfileService;
        }

        [HttpGet]
        public async Task<ActionResult<CareerProfileDto>> GetCareerProfile()
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var profile =
                await _careerProfileService.GetByUserIdAsync(userId);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        [HttpPost]
        public async Task<ActionResult<CareerProfileDto>> CreateCareerProfile(
            CreateCareerProfileDto createDto)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var createdProfile =
                await _careerProfileService.CreateAsync(
                    userId,
                    createDto);

            return CreatedAtAction(
                nameof(GetCareerProfile),
                createdProfile);
        }

        [HttpPut]
        public async Task<ActionResult<CareerProfileDto>> UpdateCareerProfile(
            UpdateCareerProfileDto updateDto)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var updatedProfile =
                await _careerProfileService.UpdateAsync(
                    userId,
                    updateDto);

            if (updatedProfile == null)
            {
                return NotFound();
            }

            return Ok(updatedProfile);
        }
    }
}