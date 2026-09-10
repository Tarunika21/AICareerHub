using System.ComponentModel.DataAnnotations;

namespace AICareerHub.API.DTOs
{
    public class ImproveResumeSummaryRequestDto
    {
        [Required]
        [MaxLength(2000)]
        public string CurrentSummary { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string TargetRole { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Skills { get; set; } = string.Empty;
    }
}