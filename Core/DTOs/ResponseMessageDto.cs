namespace Core.DTOs
{
    public class ResponseMessageDto
    {
        public string Message { get; set; }
        public List<string>? Messages { get; set; }
        public bool Status { get; set; }
    }
}
