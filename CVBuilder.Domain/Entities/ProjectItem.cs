using CVBuilder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elios.CVBuilder.Domain.Models
{
    public class ProjectItem : BaseEntity
    {
        public Guid UserCvId { get; set; }
        public string ProjectName { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string TechStack { get; set; } = default!;
        public string Achievements { get; set; } = default!;
        public string? GithubUrl { get; set; }
        public string? LiveUrl { get; set; }
        public int OrderIndex { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public UserCv UserCv { get; set; } = default!;
    }
}

