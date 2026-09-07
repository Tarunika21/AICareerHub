namespace AICareerHub.API.DTOs
{
    public class ResumeExperienceDto
    {
        public Guid Id { get; set; }

        public string Company { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public bool IsCurrent { get; set; }

        public string? Description { get; set; }
    }
}