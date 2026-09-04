using System.ComponentModel.DataAnnotations;

namespace AICareerHub.API.DTOs
{
    public class CreateCareerProfileDto
    {
        [Required]
        [MaxLength(100)]
        public string CurrentJobTitle { get; set; } = string.Empty;

        [Range(0, 60)]
        public decimal YearsOfExperience { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Skills { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string CurrentLocation { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string PreferredLocations { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string TargetRole { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal? TargetSalary { get; set; }

        [MaxLength(2000)]
        public string? ProfessionalSummary { get; set; }
    }
}