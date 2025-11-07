using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Task
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }
        public string? Category { get; set; }

        [Required]
        public string Priority { get; set; } = "Medium";

        [Required]
        public DateTime DueDate { get; set; }

        public List<long> AssigneeIds { get; set; } = new();
    }
}
