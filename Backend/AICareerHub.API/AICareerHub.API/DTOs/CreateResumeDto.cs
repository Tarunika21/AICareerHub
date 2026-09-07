using System.ComponentModel.DataAnnotations;

namespace AICareerHub.API.DTOs
{
    public class CreateResumeDto
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? ProfessionalSummary { get; set; }

        [MaxLength(1000)]
        public string Skills { get; set; } = string.Empty;
    }
}