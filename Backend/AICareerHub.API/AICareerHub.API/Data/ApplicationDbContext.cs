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
        }
    }
}