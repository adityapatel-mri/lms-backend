using System;
using System.Collections.Generic;

namespace LMS_Backend.Models;

public partial class Lead
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string Notes { get; set; } = null!;

    public int? AssignedTo { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual ICollection<LeadStatusHistory> LeadStatusHistories { get; set; } = new List<LeadStatusHistory>();
}
