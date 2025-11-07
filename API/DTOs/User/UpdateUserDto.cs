using System.ComponentModel.DataAnnotations;

namespace API.DTOs.User
{
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }
}
