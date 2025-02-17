
namespace LMS_Backend.Models.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Role { get; set; }
        public int? ReportsTo { get; set; }
       
    }
}
