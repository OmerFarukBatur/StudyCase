namespace Core.DTOs.LeaveRequestDtos
{
    public class LeaveRequestCreateDto
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeaveTypeId { get; set; }
        public string Reason { get; set; }
    }
}
