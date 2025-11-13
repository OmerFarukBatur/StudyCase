namespace Core.DTOs.EmployeeDtos
{
    public class EmployeeDashboardDto
    {
        public int TotalLeaveRequests { get; set; }
        public int ApprovedLeaves { get; set; }
        public int RejectedLeaves { get; set; }
        public int PendingLeaves { get; set; }
        public int TotalApprovedDays { get; set; }
        public string MostUsedLeaveType { get; set; }
    }
}
