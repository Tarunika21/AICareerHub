using AICareerHub.API.DTOs;

namespace AICareerHub.API.Services
{
    public interface IAiService
    {
        Task<ImproveResumeSummaryResponseDto> ImproveResumeSummaryAsync(
            ImproveResumeSummaryRequestDto request);

        Task<ImproveExperienceResponseDto> ImproveExperienceAsync(
            ImproveExperienceRequestDto request);

        Task<ResumeSuggestionsResponseDto> GetResumeSuggestionsAsync(
            ResumeSuggestionsRequestDto request);
    }
}