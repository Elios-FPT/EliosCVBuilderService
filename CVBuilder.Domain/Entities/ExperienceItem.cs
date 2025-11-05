using CVBuilder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elios.CVBuilder.Domain.Models
{
    public class ExperienceItem : BaseEntity
    {
        public Guid UserCvId { get; set; }
        public string Employer { get; set; } = default!;
        public string JobTitle { get; set; } = default!;
        public string Start { get; set; } = default!;
        public string End { get; set; } = default!;
        public string Location { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int OrderIndex { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public UserCv UserCv { get; set; } = default!;
    }
}

