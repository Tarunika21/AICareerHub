namespace AICareerHub.API.Models
{
    public class Resume
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? ProfessionalSummary { get; set; }

        public string Skills { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public User User { get; set; } = null!;

        public ICollection<ResumeExperience> Experiences { get; set; }
            = new List<ResumeExperience>();

        public ICollection<ResumeEducation> Educations { get; set; }
            = new List<ResumeEducation>();

        public ICollection<ResumeProject> Projects { get; set; }
            = new List<ResumeProject>();
    }
}