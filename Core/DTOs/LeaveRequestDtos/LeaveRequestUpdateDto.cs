namespace Core.DTOs.LeaveRequestDtos
{
    public class LeaveRequestUpdateDto
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? LeaveTypeId { get; set; }
        public string Reason { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
