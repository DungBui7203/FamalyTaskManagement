using API.DTOs.Task;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.ProfileManage
{
    public class ProfileService : IProfileService
    {
        private readonly FamilyTaskContext _context;

        public ProfileService(FamilyTaskContext context)
        {
            _context = context;
        }

        public async Task<ProfileDto?> GetProfileAsync(long userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId && u.IsActive == true)
                .Select(u => new ProfileDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<ProfileDto?> UpdateProfileAsync(long userId, UpdateProfileDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive == true);

            if (user == null)
                return null;

            // Check if email already exists for another user
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email && u.Id != userId && u.IsActive == true);

            if (emailExists)
                return null;

            user.FullName = dto.FullName;
            user.Email = dto.Email;

            await _context.SaveChangesAsync();

            return new ProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> ChangePasswordAsync(long userId, ChangePasswordDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive == true);

            if (user == null)
                return false;

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return false;

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
