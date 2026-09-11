namespace AICareerHub.API.Models
{
    public class JobApplication
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public string? JobUrl { get; set; }

        public string Location { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateOnly AppliedDate { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public User User { get; set; } = null!;
    }
}