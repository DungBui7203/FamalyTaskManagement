using System.ComponentModel.DataAnnotations;

namespace Web.Views
{
    public class ProfileViewModel
    {
        public long Id { get; set; }

        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Display(Name = "Vai trò")]
        public string Role { get; set; } = null!;

        [Display(Name = "Ngày tham gia")]
        public DateTime? CreatedAt { get; set; }
    }

    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
        [Display(Name = "Mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [Display(Name = "Mật khẩu mới")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
