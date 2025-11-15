using API.DTOs.Task;
using API.Models;
using API.Services.ProfileManage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        private long GetCurrentUserId() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        [HttpGet]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            var userId = GetCurrentUserId();
            var profile = await _profileService.GetProfileAsync(userId);

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }

        [HttpPut]
        public async Task<ActionResult<ProfileDto>> UpdateProfile(UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var profile = await _profileService.UpdateProfileAsync(userId, dto);

            if (profile == null)
                return BadRequest("Email already exists or user not found");

            return Ok(profile);
        }

        [HttpPut("password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var success = await _profileService.ChangePasswordAsync(userId, dto);

            if (!success)
                return BadRequest("Current password is incorrect or user not found");

            return Ok(new { message = "Password changed successfully" });
        }
    }
}
