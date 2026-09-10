using AICareerHub.API.AI;
using AICareerHub.API.DTOs;

namespace AICareerHub.API.Services
{
    public class AiService : IAiService
    {
        private readonly IAiProvider _aiProvider;

        public AiService(IAiProvider aiProvider)
        {
            _aiProvider = aiProvider;
        }

        public async Task<ImproveResumeSummaryResponseDto>
            ImproveResumeSummaryAsync(
                ImproveResumeSummaryRequestDto request)
        {
            var improvedSummary =
                await _aiProvider.ImproveResumeSummaryAsync(request);

            return new ImproveResumeSummaryResponseDto
            {
                ImprovedSummary = improvedSummary
            };
        }

        public async Task<ImproveExperienceResponseDto>
            ImproveExperienceAsync(
        ImproveExperienceRequestDto request)
        {
            var improvedDescription =
                await _aiProvider.ImproveExperienceAsync(request);

            return new ImproveExperienceResponseDto
            {
                ImprovedDescription = improvedDescription
            };
        }

        public async Task<ResumeSuggestionsResponseDto>
            GetResumeSuggestionsAsync(
        ResumeSuggestionsRequestDto request)
        {
            var suggestions =
                await _aiProvider.GetResumeSuggestionsAsync(request);

            return new ResumeSuggestionsResponseDto
            {
                Suggestions = suggestions
            };
        }
    }
}