using AICareerHub.API.DTOs;

namespace AICareerHub.API.AI
{
    public class MockAiProvider : IAiProvider
    {
        public Task<string> ImproveResumeSummaryAsync(
            ImproveResumeSummaryRequestDto request)
        {
            var summary = request.CurrentSummary.Trim();

            // Development-only mock. No external AI call is made.
            var result = $"[MOCK AI] {summary}";

            return Task.FromResult(result);
        }

        public Task<string> ImproveExperienceAsync(
            ImproveExperienceRequestDto request)
        {
            var result =
                $"[MOCK AI] {request.JobTitle} at {request.Company}: " +
                $"{request.CurrentDescription.Trim()}";

            return Task.FromResult(result);
        }

        public Task<List<string>> GetResumeSuggestionsAsync(
            ResumeSuggestionsRequestDto request)
        {
            var suggestions = new List<string>
            {
                "[MOCK AI] Review whether your resume highlights skills relevant to the job description.",
                "[MOCK AI] Make your experience descriptions clear and results-focused.",
                "[MOCK AI] Ensure important technologies from the job description are included only if you genuinely have experience with them."
            };

            return Task.FromResult(suggestions);
        }
    }
}