using CVBuilder.Domain.Entities;
using System;

namespace Elios.CVBuilder.Domain.Models
{
    public class UserCv : BaseEntity
    {
        public Guid OwnerId { get; set; } // user id (để kiểm quyền)
        public string ResumeTitle { get; set; } = default!;
        public string Data { get; set; } = "{}"; // nguyên JSON (y hệt file bạn gửi)
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
