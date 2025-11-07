using System;
using System.Collections.Generic;

namespace API.Models;

public partial class TaskAssignment
{
    public long Id { get; set; }

    public long TaskId { get; set; }

    public long AssigneeId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public virtual User Assignee { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;
}
