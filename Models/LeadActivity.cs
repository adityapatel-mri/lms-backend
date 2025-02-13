using System;
using System.Collections.Generic;

namespace LMS_Backend.Models;

public partial class LeadActivity
{
    public int Id { get; set; }

    public int LeadId { get; set; }

    public string? Type { get; set; }

    public int? PerformedBy { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Lead Lead { get; set; } = null!;

    public virtual User? PerformedByNavigation { get; set; }
}
