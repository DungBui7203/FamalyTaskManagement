using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Task
{
    public long Id { get; set; }

    public long FamilyId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Priority { get; set; }

    public DateTime DueDate { get; set; }

    public string? Status { get; set; }

    public long CreatedBy { get; set; }

    public long? VerifiedBy { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public bool? IsArchived { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Family Family { get; set; } = null!;

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskAttachment> TaskAttachments { get; set; } = new List<TaskAttachment>();

    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

    public virtual User? VerifiedByNavigation { get; set; }

    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();
}
