using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Label
{
    public long Id { get; set; }

    public long FamilyId { get; set; }

    public string Name { get; set; } = null!;

    public string? Color { get; set; }

    public virtual Family Family { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
