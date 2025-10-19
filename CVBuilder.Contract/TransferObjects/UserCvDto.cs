using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Contract.TransferObjects
{
    public class UserCvDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TemplateId { get; set; }
        public string Title { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CvTemplateDto? Template { get; set; }
    }
}
