using CVBuilder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elios.CVBuilder.Domain.Models
{
    public enum SkillType
    {
        Language,
        Framework,
        Tool,
        Database
    }

    public class SkillItem : BaseEntity
    {
        public Guid SkillSetId { get; set; }
        public SkillType Type { get; set; }
        public string Name { get; set; } = default!;
        public int OrderIndex { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public SkillSet SkillSet { get; set; } = default!;
    }
}

