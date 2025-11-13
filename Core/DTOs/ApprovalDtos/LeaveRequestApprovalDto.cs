namespace Core.DTOs.ApprovalDtos
{
    public class LeaveRequestApprovalDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalDays => (EndDate - StartDate).Days + 1;
        public byte[] RowVersion { get; set; }
    }
}
