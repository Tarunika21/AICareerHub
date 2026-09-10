using AICareerHub.API.DTOs;

namespace AICareerHub.API.AI
{
    public interface IAiProvider
    {
        Task<string> ImproveResumeSummaryAsync(
            ImproveResumeSummaryRequestDto request);

        Task<string> ImproveExperienceAsync(
            ImproveExperienceRequestDto request);

        Task<List<string>> GetResumeSuggestionsAsync(
            ResumeSuggestionsRequestDto request);

    }
}