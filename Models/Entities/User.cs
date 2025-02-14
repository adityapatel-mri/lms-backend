using System;
using System.Collections.Generic;

namespace LMS_Backend.Models.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? ReportsTo { get; set; }

    public virtual ICollection<User> InverseReportsToNavigation { get; set; } = new List<User>();

    public virtual ICollection<LeadActivity> LeadActivities { get; set; } = new List<LeadActivity>();

    public virtual ICollection<LeadStatusHistory> LeadStatusHistories { get; set; } = new List<LeadStatusHistory>();

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();

    public virtual User? ReportsToNavigation { get; set; }

    public virtual ICollection<UserPerformance> UserPerformances { get; set; } = new List<UserPerformance>();
}
