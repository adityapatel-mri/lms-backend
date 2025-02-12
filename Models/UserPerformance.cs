using System;
using System.Collections.Generic;

namespace LMS_Backend.Models;

public partial class UserPerformance
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? LeadsHandled { get; set; }

    public int? LeadsConverted { get; set; }

    public decimal? ResponseTimeAvg { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual User User { get; set; } = null!;
}
