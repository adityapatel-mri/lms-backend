using System;
using System.Collections.Generic;
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

    public virtual DbSet<LeadStatusHistory> LeadStatusHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPerformance> UserPerformances { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
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

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.AssignedTo, "fk_lead_assigned_to");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedTo).HasColumnName("assigned_to");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email").HasColume;
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Source)
                .HasDefaultValueSql("'Other'")
                .HasColumnType("enum('Website','Referral','Other')")
                .HasColumnName("source");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'New'")
                .HasColumnType("enum('New','Contacted','Follow-up','Converted','Lost')")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Leads)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_lead_assigned_to");
        });

        modelBuilder.Entity<LeadStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("lead_status_history");

            entity.HasIndex(e => e.LeadId, "fk_status_lead");

            entity.HasIndex(e => e.UserId, "fk_status_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("changed_at");
            entity.Property(e => e.LeadId).HasColumnName("lead_id");
            entity.Property(e => e.NewStatus)
                .HasColumnType("enum('New','Contacted','Follow-up','Converted','Lost')")
                .HasColumnName("new_status");
            entity.Property(e => e.OldStatus)
                .HasColumnType("enum('New','Contacted','Follow-up','Converted','Lost')")
                .HasColumnName("old_status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Lead).WithMany(p => p.LeadStatusHistories)
                .HasForeignKey(d => d.LeadId)
                .HasConstraintName("fk_status_lead");

            entity.HasOne(d => d.User).WithMany(p => p.LeadStatusHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_status_user");
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
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
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

            entity.HasIndex(e => e.UserId, "fk_performance_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastUpdated)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("last_updated");
            entity.Property(e => e.LeadsConverted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("leads_converted");
            entity.Property(e => e.LeadsHandled)
                .HasDefaultValueSql("'0'")
                .HasColumnName("leads_handled");
            entity.Property(e => e.ResponseTimeAvg)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("response_time_avg");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserPerformances)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_performance_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
