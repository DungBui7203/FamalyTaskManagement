using API.DTOs.Profile;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly FamilyTaskContext _context;
        public ProfileController(FamilyTaskContext context) => _context = context;

        [HttpGet("me")]
        public async Task<ActionResult<UserViewModel>> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();
            return Ok(MapToUserViewModel(user));
        }

        [HttpGet("family")]
        public async Task<ActionResult<FamilyViewModel>> GetMyFamily()
        {
            var familyId = GetCurrentFamilyId();
            var family = await _context.Families.FindAsync(familyId);
            if (family == null) return NotFound();
            var memberCount = await _context.Users.CountAsync(u => u.FamilyId == familyId);
            return Ok(MapToFamilyViewModel(family, memberCount));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !VerifyPassword(user.PasswordHash, dto.OldPassword))
                return BadRequest();
            user.PasswordHash = HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return Ok();
        }
        private long GetCurrentUserId()
        {
            // Lấy userId từ Claims hoặc HttpContext
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(userIdStr, out var userId) ? userId : 0;
        }

        private long GetCurrentFamilyId()
        {
            // Lấy familyId từ Claims hoặc HttpContext
            var familyIdStr = User.FindFirstValue("FamilyId");
            return long.TryParse(familyIdStr, out var familyId) ? familyId : 0;
        }

        private UserViewModel MapToUserViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email
            };
        }

        private FamilyViewModel MapToFamilyViewModel(Family family, int memberCount)
        {
            return new FamilyViewModel
            {
                Id = family.Id,
                Name = family.Name,
                MemberCount = memberCount
            };
        }

        private bool VerifyPassword(string hash, string password)
        {
            // Giả sử dùng BCrypt hoặc tương tự
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        private string HashPassword(string password)
        {
            // Giả sử dùng BCrypt hoặc tương tự
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        [HttpPatch("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.FullName = dto.FullName;
            user.Email = dto.Email;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
