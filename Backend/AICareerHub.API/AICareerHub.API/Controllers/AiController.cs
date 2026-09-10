using AICareerHub.API.DTOs;
using AICareerHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AICareerHub.API.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("resume-summary")]
        public async Task<ActionResult<ImproveResumeSummaryResponseDto>>
            ImproveResumeSummary(
                ImproveResumeSummaryRequestDto request)
        {
            var result =
                await _aiService.ImproveResumeSummaryAsync(request);

            return Ok(result);
        }

        [HttpPost("experience")]
        public async Task<ActionResult<ImproveExperienceResponseDto>>
        ImproveExperience(ImproveExperienceRequestDto request)
        {
            var result = await _aiService.ImproveExperienceAsync(request);

            return Ok(result);
        }

        [HttpPost("resume-suggestions")]
        public async Task<ActionResult<ResumeSuggestionsResponseDto>>
        GetResumeSuggestions(ResumeSuggestionsRequestDto request)
        {
            var result =
                await _aiService.GetResumeSuggestionsAsync(request);

            return Ok(result);
        }
    }
}