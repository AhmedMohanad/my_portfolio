// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Portofolio.Models.ImageModel;
using Portofolio.Models.UserModels;

namespace Portofolio.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<ImageData> ImageDatas { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.Email).IsUnique();
                entity.Property(p => p.FullName).IsRequired().HasMaxLength(75);
                entity.Property(p => p.Email).IsRequired().HasMaxLength(100);
                entity.Property(p => p.PasswordHash).IsRequired();
                entity.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(p => p.Bio).HasMaxLength(500);
            });

            modelBuilder.Entity<Experience>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Company).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.StartDate).IsRequired();

                entity.HasOne(e => e.Profile)
                    .WithMany(p => p.Experiences)
                    .HasForeignKey(e => e.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ProfileId);
            });

            modelBuilder.Entity<ImageData>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Url).IsRequired().HasMaxLength(500);
                entity.Property(i => i.UploadedAt).IsRequired();

                entity.HasOne(i => i.Profile)
                    .WithMany(p => p.ProfilePictures)  
                    .HasForeignKey(i => i.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(i => i.ProfileId);
                entity.HasIndex(i => i.UploadedAt);
            });
        }
    }
}