using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Task
{
    public class UpdateTaskStatusDto
    {
        [Required]
        public string Status { get; set; } = null!;
    }
}
