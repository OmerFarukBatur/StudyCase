namespace Core.DTOs.AdminDtos
{
    public class MonthlyLeaveSummaryDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalApprovedDays { get; set; }
    }
}
