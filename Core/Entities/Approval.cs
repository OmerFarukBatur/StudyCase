using Core.Entities.Common;
using Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Approval : BaseEntity
    {
        public int LeaveRequestId { get; set; }
        public int ManagerId { get; set; }
        public string? Comments { get; set; }
        public int? ApprovalActionId { get; set; }
        [NotMapped]
        public ApprovalAction ApprovalAction
        {
            get => (ApprovalAction)ApprovalActionId;
            set => ApprovalActionId = (int)value;
        }

        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual LeaveRequest LeaveRequest { get; set; }
        public virtual User Manager { get; set; }
        public virtual User CreatedUser { get; set; }
        public virtual User UpdatedUser { get; set; }
    }
}
