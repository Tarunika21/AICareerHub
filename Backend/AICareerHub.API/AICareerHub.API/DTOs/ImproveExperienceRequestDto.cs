using System.ComponentModel.DataAnnotations;

namespace AICareerHub.API.DTOs
{
    public class ImproveExperienceRequestDto
    {
        [Required]
        [MaxLength(150)]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Company { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string CurrentDescription { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Technologies { get; set; }
    }
}