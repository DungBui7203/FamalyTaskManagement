using System;
using System.Collections.Generic;

namespace API.Models;

public partial class TaskAttachment
{
    public long Id { get; set; }

    public long TaskId { get; set; }

    public string? FileName { get; set; }

    public string? Url { get; set; }

    public long UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User UploadedByNavigation { get; set; } = null!;
}
