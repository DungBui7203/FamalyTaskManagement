using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class FamilyTaskContext : DbContext
{
    public FamilyTaskContext()
    {
    }

    public FamilyTaskContext(DbContextOptions<FamilyTaskContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<Family> Families { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskAssignment> TaskAssignments { get; set; }

    public virtual DbSet<TaskAttachment> TaskAttachments { get; set; }

    public virtual DbSet<TaskComment> TaskComments { get; set; }

    public virtual DbSet<TimeEntry> TimeEntries { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string ConnectionStr = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(ConnectionStr);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__activity__3213E83FE0E95989");

            entity.ToTable("activity_logs");

            entity.HasIndex(e => new { e.FamilyId, e.CreatedAt }, "IX_activity_logs_family_created");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.ActorId).HasColumnName("actor_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DataJson).HasColumnName("data_json");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.FamilyId).HasColumnName("family_id");

            entity.HasOne(d => d.Actor).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.ActorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_activity_logs_actor");

            entity.HasOne(d => d.Family).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.FamilyId)
                .HasConstraintName("FK_activity_logs_family");
        });

        modelBuilder.Entity<Family>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__families__3213E83F44C52995");

            entity.ToTable("families");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__labels__3213E83F3B7B06A1");

            entity.ToTable("labels");

            entity.HasIndex(e => new { e.FamilyId, e.Name }, "UQ_labels_family_name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Color)
                .HasMaxLength(20)
                .HasColumnName("color");
            entity.Property(e => e.FamilyId).HasColumnName("family_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.Family).WithMany(p => p.Labels)
                .HasForeignKey(d => d.FamilyId)
                .HasConstraintName("FK_labels_family");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tasks__3213E83F512E2804");

            entity.ToTable("tasks");

            entity.HasIndex(e => new { e.FamilyId, e.Status, e.DueDate }, "IX_tasks_family_status_due");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DueDate)
                .HasPrecision(0)
                .HasColumnName("due_date");
            entity.Property(e => e.FamilyId).HasColumnName("family_id");
            entity.Property(e => e.IsArchived)
                .HasDefaultValue(false)
                .HasColumnName("is_archived");
            entity.Property(e => e.Priority)
                .HasMaxLength(10)
                .HasDefaultValue("Medium")
                .HasColumnName("priority");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("updated_at");
            entity.Property(e => e.VerifiedAt)
                .HasPrecision(0)
                .HasColumnName("verified_at");
            entity.Property(e => e.VerifiedBy).HasColumnName("verified_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tasks_created_by");

            entity.HasOne(d => d.Family).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.FamilyId)
                .HasConstraintName("FK_tasks_families");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.TaskVerifiedByNavigations)
                .HasForeignKey(d => d.VerifiedBy)
                .HasConstraintName("FK_tasks_verified_by");

            entity.HasMany(d => d.Labels).WithMany(p => p.Tasks)
                .UsingEntity<Dictionary<string, object>>(
                    "TaskLabel",
                    r => r.HasOne<Label>().WithMany()
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_task_labels_label"),
                    l => l.HasOne<Task>().WithMany()
                        .HasForeignKey("TaskId")
                        .HasConstraintName("FK_task_labels_task"),
                    j =>
                    {
                        j.HasKey("TaskId", "LabelId");
                        j.ToTable("task_labels");
                        j.IndexerProperty<long>("TaskId").HasColumnName("task_id");
                        j.IndexerProperty<long>("LabelId").HasColumnName("label_id");
                    });
        });

        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__task_ass__3213E83FA7F869F4");

            entity.ToTable("task_assignments");

            entity.HasIndex(e => new { e.AssigneeId, e.TaskId }, "IX_task_assignments_assignee_task");

            entity.HasIndex(e => new { e.TaskId, e.AssigneeId }, "UQ_task_assignments").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("assigned_at");
            entity.Property(e => e.AssigneeId).HasColumnName("assignee_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Assignee).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.AssigneeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_task_assignments_user");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_task_assignments_task");
        });

        modelBuilder.Entity<TaskAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__task_att__3213E83F3811C559");

            entity.ToTable("task_attachments");

            entity.HasIndex(e => new { e.TaskId, e.UploadedAt }, "IX_task_attachments_task_uploaded");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UploadedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("uploaded_at");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");
            entity.Property(e => e.Url)
                .HasMaxLength(512)
                .HasColumnName("url");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAttachments)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_task_attachments_task");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.TaskAttachments)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_task_attachments_user");
        });

        modelBuilder.Entity<TaskComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__task_com__3213E83F68F08BD5");

            entity.ToTable("task_comments");

            entity.HasIndex(e => new { e.TaskId, e.CreatedAt }, "IX_task_comments_task_created");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(2000)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskComments)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_task_comments_task");

            entity.HasOne(d => d.User).WithMany(p => p.TaskComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_task_comments_user");
        });

        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__time_ent__3213E83F94B2FE50");

            entity.ToTable("time_entries");

            entity.HasIndex(e => new { e.TaskId, e.UserId }, "IX_time_entries_task_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LoggedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("logged_at");
            entity.Property(e => e.Minutes).HasColumnName("minutes");
            entity.Property(e => e.Note)
                .HasMaxLength(1000)
                .HasColumnName("note");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TimeEntries)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_time_entries_task");

            entity.HasOne(d => d.User).WithMany(p => p.TimeEntries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_time_entries_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FAACB7A61");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FamilyId).HasColumnName("family_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");

            entity.HasOne(d => d.Family).WithMany(p => p.Users)
                .HasForeignKey(d => d.FamilyId)
                .HasConstraintName("FK_users_families");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
