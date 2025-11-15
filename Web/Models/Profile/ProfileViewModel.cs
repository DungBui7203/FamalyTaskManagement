using Web.Models.Task;

namespace Web.Models.Profile
{
    public class ProfileViewModel
    {
        public ProfileUserViewModel? User { get; set; }
        public FamilyViewModel? Family { get; set; }
    }
    public class ChangePasswordViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class ProfileUserViewModel
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class FamilyViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MemberCount { get; set; }
    }
}
