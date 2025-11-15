using API.DTOs.Task;

namespace API.Services.ProfileManage
{
    public interface IProfileService
    {
        Task<ProfileDto?> GetProfileAsync(long userId);
        Task<ProfileDto?> UpdateProfileAsync(long userId, UpdateProfileDto dto);
        Task<bool> ChangePasswordAsync(long userId, ChangePasswordDto dto);
    }
}
