using Core.Entities.Common;
using Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class LeaveRequest : BaseEntity
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? LeaveTypeId { get; set; }
        [NotMapped]
        public LeaveType LeaveType
        {
            get => (LeaveType)LeaveTypeId;
            set => LeaveTypeId = (int)value;
        }
        public string Reason { get; set; }
        public int? LeaveStatusId { get; set; }
        [NotMapped]
        public LeaveStatus LeaveStatus
        {
            get => (LeaveStatus)LeaveStatusId;
            set => LeaveStatusId = (int)value;
        }
        public string? RejectionReason { get; set; }

        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }


        public virtual User User { get; set; }
        public virtual User CreatedUser { get; set; }
        public virtual User UpdatedUser { get; set; }
        public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    }
}
