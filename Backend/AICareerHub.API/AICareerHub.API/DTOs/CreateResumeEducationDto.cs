using System.ComponentModel.DataAnnotations;

namespace AICareerHub.API.DTOs
{
    public class CreateResumeEducationDto
    {
        [Required]
        [MaxLength(200)]
        public string Institution { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Degree { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? FieldOfStudy { get; set; }

        [Range(1950, 2100)]
        public int StartYear { get; set; }

        [Range(1950, 2100)]
        public int? EndYear { get; set; }
    }
}