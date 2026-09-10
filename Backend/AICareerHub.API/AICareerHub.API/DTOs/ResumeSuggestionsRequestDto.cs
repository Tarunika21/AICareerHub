using System.ComponentModel.DataAnnotations;

namespace AICareerHub.API.DTOs
{
    public class ResumeSuggestionsRequestDto
    {
        [Required]
        [MaxLength(3000)]
        public string ResumeContent { get; set; } = string.Empty;

        [Required]
        [MaxLength(5000)]
        public string JobDescription { get; set; } = string.Empty;
    }
}