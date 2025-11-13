namespace Core.DTOs.LeaveRequestDtos
{
    public class LeaveRequestApprovalDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; }
        public string ApprovalAction { get; set; }        
        public int? ApprovalActionId { get; set; }
        public string ManagerName { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalDays => (EndDate - StartDate).Days + 1;
    }
}
