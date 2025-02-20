namespace LMS_Backend.Models.DTOs
{
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
