using System;
using System.Collections.Generic;
using LMS_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace LMS_Backend.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Lead> Leads { get; set; }

    public virtual DbSet<LeadActivity> LeadActivities { get; set; }

    public virtual DbSet<LeadStatusHistory> LeadStatusHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPerformance> UserPerformances { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("server=localhost;database=lms;user=root;password=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.4.4-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("leads");

            entity.HasIndex(e => e.AssignedTo, "leads_assigned_to_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedTo).HasColumnName("assigned_to");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Source)
                .HasDefaultValueSql("'Other'")
                .HasColumnType("enum('Website','Referral','Other')")
                .HasColumnName("source");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'New'")
                .HasColumnType("enum('New','Contacted','Follow-Up','Converted','Lost')")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Leads)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("leads_assigned_to_foreign");
        });

        modelBuilder.Entity<LeadActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("lead_activity");

            entity.HasIndex(e => e.LeadId, "lead_activity_lead_id_foreign");

            entity.HasIndex(e => e.PerformedBy, "lead_activity_performed_by_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.LeadId).HasColumnName("lead_id");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.PerformedBy).HasColumnName("performed_by");
            entity.Property(e => e.Type)
                .HasDefaultValueSql("'Other'")
                .HasColumnType("enum('Call','Email','InPerson','Other')")
                .HasColumnName("type");

            entity.HasOne(d => d.Lead).WithMany(p => p.LeadActivities)
                .HasForeignKey(d => d.LeadId)
                .HasConstraintName("lead_activity_lead_id_foreign");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.LeadActivities)
                .HasForeignKey(d => d.PerformedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("lead_activity_performed_by_foreign");
        });

        modelBuilder.Entity<LeadStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("lead_status_history");

            entity.HasIndex(e => e.ChangedBy, "lead_status_history_changed_by_foreign");

            entity.HasIndex(e => e.LeadId, "lead_status_history_lead_id_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.LeadId).HasColumnName("lead_id");
            entity.Property(e => e.NewStatus)
                .HasColumnType("enum('New','Contacted','Follow-Up','Converted','Lost')")
                .HasColumnName("new_status");
            entity.Property(e => e.OldStatus)
                .HasColumnType("enum('New','Contacted','Follow-Up','Converted','Lost')")
                .HasColumnName("old_status");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.LeadStatusHistories)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("lead_status_history_changed_by_foreign");

            entity.HasOne(d => d.Lead).WithMany(p => p.LeadStatusHistories)
                .HasForeignKey(d => d.LeadId)
                .HasConstraintName("lead_status_history_lead_id_foreign");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'Sales'")
                .HasColumnType("enum('Admin','Manager','Sales')")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserPerformance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_performance");

            entity.HasIndex(e => e.UserId, "user_performance_user_id_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastUpdated)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("last_updated");
            entity.Property(e => e.LeadsAssigned)
                .HasDefaultValueSql("'0'")
                .HasColumnName("leads_assigned");
            entity.Property(e => e.LeadsConverted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("leads_converted");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserPerformances)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_performance_user_id_foreign");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
