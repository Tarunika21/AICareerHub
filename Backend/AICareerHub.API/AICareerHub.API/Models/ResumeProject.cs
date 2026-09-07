namespace AICareerHub.API.Models
{
    public class ResumeProject
    {
        public Guid Id { get; set; }

        public Guid ResumeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Technologies { get; set; } = string.Empty;

        public string? ProjectUrl { get; set; }

        public Resume Resume { get; set; } = null!;
    }
}