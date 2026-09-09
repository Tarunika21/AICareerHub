namespace AICareerHub.API.DTOs
{
    public class ResumeEducationDto
    {
        public Guid Id { get; set; }

        public string Institution { get; set; } = string.Empty;

        public string Degree { get; set; } = string.Empty;

        public string? FieldOfStudy { get; set; }

        public int StartYear { get; set; }

        public int? EndYear { get; set; }
    }
}