using CVBuilder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elios.CVBuilder.Domain.Models
{
    public class Link : BaseEntity
    {
        public Guid PersonalInfoId { get; set; }
        public string Name { get; set; } = default!;
        public string Url { get; set; } = default!;

        // Navigation properties
        public PersonalInfo PersonalInfo { get; set; } = default!;
    }
}

