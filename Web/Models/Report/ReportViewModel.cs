namespace Web.Models.Report
{
    public class ReportViewModel
    {
        public List<TaskMemberStat> MemberStats { get; set; } = new();
        public List<OverdueTaskGroup> OverdueGroups { get; set; } = new();
    }
    public class TaskMemberStat
    {
        public long UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int AssignedCount { get; set; }
        public int PendingCount { get; set; }
        public int InProgressCount { get; set; }
        public int DoneCount { get; set; }
        public int OnTimeCount { get; set; }
        public int LateCount { get; set; }
    }

    public class OverdueTask
    {
        public long TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int DaysOverdue { get; set; }
    }

    public class OverdueTaskGroup
    {
        public long AssigneeId { get; set; }
        public string AssigneeName { get; set; } = string.Empty;
        public List<OverdueTask> OverdueTasks { get; set; } = new();
        public int TotalOverdue { get; set; }
    }
}
