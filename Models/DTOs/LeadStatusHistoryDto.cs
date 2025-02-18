namespace LMS_Backend.Models.DTOs
{
    public class LeadStatusHistoryDto
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public int? ChangedBy { get; set; }
    }
}

