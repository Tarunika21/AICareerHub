namespace AICareerHub.API.DTOs
{
    public class ResumeDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? ProfessionalSummary { get; set; }

        public string Skills { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}