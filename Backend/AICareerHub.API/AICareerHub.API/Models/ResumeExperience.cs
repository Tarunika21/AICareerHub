namespace AICareerHub.API.Models
{
    public class ResumeExperience
    {
        public Guid Id { get; set; }

        public Guid ResumeId { get; set; }

        public string Company { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public bool IsCurrent { get; set; }

        public string? Description { get; set; }

        public Resume Resume { get; set; } = null!;
    }
}