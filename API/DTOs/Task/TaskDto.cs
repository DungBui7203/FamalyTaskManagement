namespace API.DTOs.Task
{
    public class TaskDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string Priority { get; set; } = "Medium";
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = "Pending";
        public long CreatedBy { get; set; }
        public string CreatedByName { get; set; } = null!;
        public long? VerifiedBy { get; set; }
        public string? VerifiedByName { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> AssigneeNames { get; set; } = new();
        public List<long> AssigneeIds { get; set; } = new();
        public List<TaskAssignmentDto> Assignments { get; set; } = new(); // Thêm dòng này
    }

    public class TaskAssignmentDto
    {
        public long AssigneeId { get; set; }
        public string AssigneeName { get; set; } = null!;
        public string Status { get; set; } = "Pending";
        public DateTime? AssignedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
