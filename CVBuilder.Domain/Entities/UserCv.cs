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
        public string Title { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public TemplateCv Template { get; set; } = default!;
    }
}
