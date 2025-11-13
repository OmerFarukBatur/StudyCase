namespace Core.DTOs.LeaveRequestDtos
{
    public class LeaveRequestListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
        public int? LeaveStatusId { get; set; }
        public string LeaveStatus { get; set; }
        public string? RejectionReason { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
