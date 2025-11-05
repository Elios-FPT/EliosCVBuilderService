using Microsoft.EntityFrameworkCore;
using System;

namespace CVBuilder.Infrastructure.DataContext
{
    public class CVBuilderDataContext : DbContext
    {
        public CVBuilderDataContext(DbContextOptions<CVBuilderDataContext> options) : base(options)
        {
        }

        public virtual DbSet<Elios.CVBuilder.Domain.Models.TemplateCv> TemplateCvs { get; set; }
        public virtual DbSet<Elios.CVBuilder.Domain.Models.UserCv> UserCvs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TemplateCv (separate feature, keep for template management)
            modelBuilder.Entity<Elios.CVBuilder.Domain.Models.TemplateCv>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
            });

            // Configure UserCv - single table with JSON body storage
            modelBuilder.Entity<Elios.CVBuilder.Domain.Models.UserCv>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.OwnerId)
                    .IsRequired();
                
                entity.Property(e => e.Body)
                    .IsRequired()
                    .HasColumnType("jsonb");
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt);
                
                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);
                
                entity.Property(e => e.DeletedAt);
                
                // Index for filtering by owner
                entity.HasIndex(e => e.OwnerId)
                    .HasDatabaseName("IX_UserCv_OwnerId");
                
                // Global query filter for soft-delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }
    }
}
