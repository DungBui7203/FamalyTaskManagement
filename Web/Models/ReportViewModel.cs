using Web.Models.Task;

namespace Web.Models
{
    public class ReportViewModel
    {
        public List<UserViewModel> Members { get; set; }
        public List<TaskReportItem> Report { get; set; }
    }
    public class TaskReportItem
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public int AssignedCount { get; set; }
        public int InProgressCount { get; set; }
        public int DoneCount { get; set; }
        public int OnTimeCount { get; set; }
        public int LateCount { get; set; }
    }
}
