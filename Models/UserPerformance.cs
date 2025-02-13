using System;
using System.Collections.Generic;

namespace LMS_Backend.Models;

public partial class UserPerformance
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public long? LeadsAssigned { get; set; }

    public long? LeadsConverted { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual User User { get; set; } = null!;
}
