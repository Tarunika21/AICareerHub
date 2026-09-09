using System.ComponentModel.DataAnnotations;

namespace AICareerHub.API.DTOs
{
    public class CreateResumeProjectDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Technologies { get; set; } = string.Empty;

        [MaxLength(500)]
        [Url]
        public string? ProjectUrl { get; set; }
    }
}