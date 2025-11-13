using Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? RoleId { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual Role Role { get; set; }
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
