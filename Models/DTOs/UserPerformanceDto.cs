namespace LMS_Backend.Models.DTOs
{
    public class UserPerformanceDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public long? LeadsAssigned { get; set; }
        public long? LeadsConverted { get; set; }
    }
}

