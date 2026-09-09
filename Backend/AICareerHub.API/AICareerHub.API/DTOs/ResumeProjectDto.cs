namespace AICareerHub.API.DTOs
{
    public class ResumeProjectDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Technologies { get; set; } = string.Empty;

        public string? ProjectUrl { get; set; }
    }
}