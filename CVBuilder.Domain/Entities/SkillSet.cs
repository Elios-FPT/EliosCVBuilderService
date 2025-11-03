using CVBuilder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elios.CVBuilder.Domain.Models
{
    public class SkillSet : BaseEntity
    {
        public Guid UserCvId { get; set; }
        public string SectionId { get; set; } = "skillsets";
        public string Title { get; set; } = "Skillsets";

        // Navigation properties
        public UserCv UserCv { get; set; } = default!;
        public ICollection<SkillItem> SkillItems { get; set; } = new List<SkillItem>();
    }
}
