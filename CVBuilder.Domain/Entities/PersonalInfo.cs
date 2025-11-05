using CVBuilder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elios.CVBuilder.Domain.Models
{
    public class PersonalInfo : BaseEntity
    {
        public Guid UserCvId { get; set; }
        public string Title { get; set; } = "Personal Info";
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string JobTitle { get; set; } = default!;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public UserCv UserCv { get; set; } = default!;
        public ICollection<Link> Links { get; set; } = new List<Link>();
    }
}
