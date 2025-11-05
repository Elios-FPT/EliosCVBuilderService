using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // Configure TemplateCv
            modelBuilder.Entity<Elios.CVBuilder.Domain.Models.TemplateCv>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
            });

            // Configure UserCv
            modelBuilder.Entity<Elios.CVBuilder.Domain.Models.UserCv>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ResumeTitle).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);
            });
        }
    }
}
