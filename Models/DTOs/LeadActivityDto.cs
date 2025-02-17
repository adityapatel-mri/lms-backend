using System;

namespace LMS_Backend.Models.DTOs
{
    public class LeadActivityDto
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public string? Type { get; set; }
        public int? PerformedBy { get; set; }
        public string? Notes { get; set; }
    }
}
