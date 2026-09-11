using System.ComponentModel.DataAnnotations;

namespace AICareerHub.API.DTOs
{
    public class CreateJobApplicationDto
    {
        [Required]
        [MaxLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string JobTitle { get; set; } = string.Empty;

        [Url]
        [MaxLength(500)]
        public string? JobUrl { get; set; }

        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public DateOnly AppliedDate { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }
    }
}