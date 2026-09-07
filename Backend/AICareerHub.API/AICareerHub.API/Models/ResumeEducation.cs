namespace AICareerHub.API.Models
{
    public class ResumeEducation
    {
        public Guid Id { get; set; }

        public Guid ResumeId { get; set; }

        public string Institution { get; set; } = string.Empty;

        public string Degree { get; set; } = string.Empty;

        public string? FieldOfStudy { get; set; }

        public int StartYear { get; set; }

        public int? EndYear { get; set; }

        public Resume Resume { get; set; } = null!;
    }
}