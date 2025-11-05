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
        public string Title { get; set; } = "Skillsets";
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public UserCv UserCv { get; set; } = default!;
        public ICollection<SkillItem> SkillItems { get; set; } = new List<SkillItem>();
    }
}
