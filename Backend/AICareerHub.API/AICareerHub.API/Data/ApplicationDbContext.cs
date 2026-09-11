using Microsoft.EntityFrameworkCore;
using AICareerHub.API.Models;

namespace AICareerHub.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<CareerProfile> CareerProfiles { get; set; }

        public DbSet<Resume> Resumes { get; set; }

        public DbSet<ResumeExperience> ResumeExperiences { get; set; }

        public DbSet<ResumeEducation> ResumeEducations { get; set; }

        public DbSet<ResumeProject> ResumeProjects { get; set; }

        public DbSet<JobApplication> JobApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();

            modelBuilder.Entity<CareerProfile>()
                .HasOne(profile => profile.User)
                .WithOne(user => user.CareerProfile)
                .HasForeignKey<CareerProfile>(
                    profile => profile.UserId);

            modelBuilder.Entity<Resume>()
                .HasOne(resume => resume.User)
                .WithMany(user => user.Resumes)
                .HasForeignKey(resume => resume.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResumeExperience>()
                .HasOne(experience => experience.Resume)
                .WithMany(resume => resume.Experiences)
                .HasForeignKey(experience => experience.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResumeEducation>()
                .HasOne(education => education.Resume)
                .WithMany(resume => resume.Educations)
                .HasForeignKey(education => education.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResumeProject>()
                .HasOne(project => project.Resume)
                .WithMany(resume => resume.Projects)
                .HasForeignKey(project => project.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobApplication>()
                .HasOne(job => job.User)
                .WithMany(user => user.JobApplications)
                .HasForeignKey(job => job.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobApplication>()
                .Property(job => job.CompanyName)
                .HasMaxLength(150);

            modelBuilder.Entity<JobApplication>()
                .Property(job => job.JobTitle)
                .HasMaxLength(150);

            modelBuilder.Entity<JobApplication>()
                .Property(job => job.Location)
                .HasMaxLength(150);

            modelBuilder.Entity<JobApplication>()
                .Property(job => job.Status)
                .HasMaxLength(50);

            modelBuilder.Entity<JobApplication>()
                .Property(job => job.JobUrl)
                .HasMaxLength(500);

            modelBuilder.Entity<JobApplication>()
                .Property(job => job.Notes)
                .HasMaxLength(2000);
        }
    }
}