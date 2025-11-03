using CVBuilder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elios.CVBuilder.Domain.Models
{
    public class EducationItem : BaseEntity
    {
        public Guid UserCvId { get; set; }
        public string Institution { get; set; } = default!;
        public string Location { get; set; } = default!;
        public string DegreeType { get; set; } = default!;
        public string FieldOfStudy { get; set; } = default!;
        public string Start { get; set; } = default!;
        public string Grad { get; set; } = default!;
        public string? Gpa { get; set; }
        public int OrderIndex { get; set; }

        // Navigation properties
        public UserCv UserCv { get; set; } = default!;
    }
}

