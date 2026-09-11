namespace AICareerHub.API.DTOs
{
    public class JobApplicationStatsDto
    {
        public int Total { get; set; }

        public int Applied { get; set; }

        public int Interview { get; set; }

        public int Offer { get; set; }

        public int Rejected { get; set; }

        public int Withdrawn { get; set; }
    }
}