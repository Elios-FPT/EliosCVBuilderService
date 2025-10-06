using Microsoft.EntityFrameworkCore;
using EliosCVBuilderService.Models;

namespace EliosCVBuilderService.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<UserCV> UserCVs { get; set; }
        public DbSet<TemplateCV> TemplateCVs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserCV>()
                .HasOne(u => u.Template)
                .WithMany(t => t.UserCVs)
                .HasForeignKey(u => u.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserCV>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<UserCV>()
                .Property(u => u.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<TemplateCV>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<TemplateCV>()
                .Property(t => t.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}
