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
        }
    }
}