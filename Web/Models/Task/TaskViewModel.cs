using System.ComponentModel.DataAnnotations;

namespace Web.Models.Task
{
    public class TaskViewModel
    {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = null!;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Danh mục")]
        public string? Category { get; set; }

        [Required]
        [Display(Name = "Độ ưu tiên")]
        public string Priority { get; set; } = "Medium";

        [Required]
        [Display(Name = "Hạn chót")]
        [DataType(DataType.DateTime)]
        public DateTime DueDate { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Người tạo")]
        public string CreatedByName { get; set; } = null!;

        [Display(Name = "Người phân công")]
        public List<long> AssigneeIds { get; set; } = new();

        public List<string> AssigneeNames { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TaskListViewModel
    {
        public List<TaskViewModel> Tasks { get; set; } = new();
        public List<UserViewModel> FamilyMembers { get; set; } = new();
        public string? StatusFilter { get; set; }
        public long? AssigneeFilter { get; set; }
    }

    public class UserViewModel
    {
        public long Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
