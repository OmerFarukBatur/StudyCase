namespace Core.DTOs.ApprovalDtos
{
    public class ApproveRejectDto
    {
        public int LeaveRequestId { get; set; }
        public bool IsApproved { get; set; }
        public string Comments { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
