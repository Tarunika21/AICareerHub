using System.ComponentModel.DataAnnotations;

namespace AICareerHub.API.DTOs
{
    public class UpdateResumeExperienceDto
    {
        [Required]
        [MaxLength(150)]
        public string Company { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string JobTitle { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public bool IsCurrent { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }
    }
}