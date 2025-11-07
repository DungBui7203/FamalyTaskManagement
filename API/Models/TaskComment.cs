using System;
using System.Collections.Generic;

namespace API.Models;

public partial class TaskComment
{
    public long Id { get; set; }

    public long TaskId { get; set; }

    public long UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
