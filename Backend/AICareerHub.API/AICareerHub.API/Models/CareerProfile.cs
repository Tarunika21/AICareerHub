namespace AICareerHub.API.Models
{
    public class CareerProfile
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string CurrentJobTitle { get; set; } = string.Empty;

        public decimal YearsOfExperience { get; set; }

        public string Skills { get; set; } = string.Empty;

        public string CurrentLocation { get; set; } = string.Empty;

        public string PreferredLocations { get; set; } = string.Empty;

        public string TargetRole { get; set; } = string.Empty;

        public decimal? TargetSalary { get; set; }

        public string? ProfessionalSummary { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public User User { get; set; } = null!;
    }
}