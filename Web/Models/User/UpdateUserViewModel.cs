using System.ComponentModel.DataAnnotations;

namespace Web.Models.User
{
    public class UpdateUserViewModel
    {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = null!;

        [Required]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = null!;
    }
}
