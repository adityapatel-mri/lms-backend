using System;
using System.Collections.Generic;

namespace LMS_Backend.Models;

public partial class LeadStatusHistory
{
    public int Id { get; set; }

    public int LeadId { get; set; }

    public int? UserId { get; set; }

    public string OldStatus { get; set; } = null!;

    public string NewStatus { get; set; } = null!;

    public DateTime? ChangedAt { get; set; }

    public virtual Lead Lead { get; set; } = null!;

    public virtual User? User { get; set; }
}
