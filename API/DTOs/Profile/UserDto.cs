namespace API.DTOs.Profile
{
    public class UserDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
    public class FamilyDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int MemberCount { get; set; }
    }
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class TaskReportItemDto
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public int AssignedCount { get; set; }
        public int InProgressCount { get; set; }
        public int DoneCount { get; set; }
        public int OnTimeCount { get; set; }
        public int LateCount { get; set; }
    }
    public class UserViewModel
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    public class FamilyViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int MemberCount { get; set; }
    }
    public class UpdateProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

}
