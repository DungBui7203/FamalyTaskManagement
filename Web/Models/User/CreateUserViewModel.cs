using System.ComponentModel.DataAnnotations;

namespace Web.Models.User
{
    public class CreateUserViewModel
    {
        [Required]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "Member";
    }
}
