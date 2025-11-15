namespace API.DTOs
{
    public class OverdueTaskDto
    {
        public long TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public long AssigneeId { get; set; }
        public string AssigneeName { get; set; } = string.Empty;
        public int DaysOverdue { get; set; }
    }

    public class OverdueTaskGroupDto
    {
        public long AssigneeId { get; set; }
        public string AssigneeName { get; set; } = string.Empty;
        public List<OverdueTaskDto> OverdueTasks { get; set; } = new();
        public int TotalOverdue { get; set; }
    }
}
