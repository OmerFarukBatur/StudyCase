namespace Core.DTOs.UserDtos
{
    public class PasswordResetDto
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}
