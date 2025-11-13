using Core.Enums;

namespace Core.DTOs.ApprovalDtos
{
    public class ApprovalCreateDto
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        public string Reason { get; set; }
        public byte[] RowVersion { get; set; }

        public string? Comments { get; set; } = string.Empty;
        public int ApprovalActionId { get; set; }

        public bool IsApproved => ApprovalActionId == (int)ApprovalAction.Approve;
    }
}
