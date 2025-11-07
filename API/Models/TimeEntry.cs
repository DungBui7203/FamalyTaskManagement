using System;
using System.Collections.Generic;

namespace API.Models;

public partial class TimeEntry
{
    public long Id { get; set; }

    public long TaskId { get; set; }

    public long UserId { get; set; }

    public int? Minutes { get; set; }

    public string? Note { get; set; }

    public DateTime? LoggedAt { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
