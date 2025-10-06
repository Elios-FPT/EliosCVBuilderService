using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EliosCVBuilderService.Models
{
    [Table("UserCV")]
    public class UserCV
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CvId { get; set; }

        [Required]
        public long UserId { get; set; }

        public long? TemplateId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("TemplateId")]
        public TemplateCV? Template { get; set; }
    }
}
