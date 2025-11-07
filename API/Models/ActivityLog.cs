using System;
using System.Collections.Generic;

namespace API.Models;

public partial class ActivityLog
{
    public long Id { get; set; }

    public long FamilyId { get; set; }

    public string? EntityType { get; set; }

    public long? EntityId { get; set; }

    public string? Action { get; set; }

    public long ActorId { get; set; }

    public string? DataJson { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Actor { get; set; } = null!;

    public virtual Family Family { get; set; } = null!;
}
