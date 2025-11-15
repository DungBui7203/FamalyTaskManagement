namespace API.DTOs
{
    public class TaskMemberStatDto
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
}
