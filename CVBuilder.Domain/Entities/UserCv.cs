using CVBuilder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elios.CVBuilder.Domain.Models
{
    public class UserCv : BaseEntity
    {
        public Guid UserId { get; set; }      
        public Guid TemplateId { get; set; }
        public string ResumeTitle { get; set; } = default!;
        public TemplateCv Template { get; set; } = default!;
        public PersonalInfo? PersonalInfo { get; set; }
        public ICollection<ExperienceItem> ExperienceItems { get; set; } = new List<ExperienceItem>();
        public ICollection<ProjectItem> ProjectItems { get; set; } = new List<ProjectItem>();
        public ICollection<EducationItem> EducationItems { get; set; } = new List<EducationItem>();
        public SkillSet? SkillSet { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
