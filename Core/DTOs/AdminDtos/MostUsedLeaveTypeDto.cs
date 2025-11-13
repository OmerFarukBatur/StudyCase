namespace Core.DTOs.AdminDtos
{
    public class MostUsedLeaveTypeDto
    {
        public string TypeName { get; set; }
        public int UsageCount { get; set; }
    }

    public class DashboardSummaryViewModel
    {
        public int ApprovedLeavesThisMonth { get; set; }
        public MostUsedLeaveTypeDto MostUsedLeaveType { get; set; }
        public int PendingRequestsCount { get; set; }
        public int TotalEmployees { get; set; }
        public int ThisWeekLeaves { get; set; }
        public int RejectedLeavesThisMonth { get; set; }
        public double AverageLeaveDays { get; set; }
        public int UpcomingLeaves { get; set; }
        public double AnnualLeaveUsageRate { get; set; }
    }
}
